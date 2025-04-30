using System.Text;
using System.Text.Json;
using AngleSharp.Html.Parser;
using EsportsApi.Application.Contracts;
using EsportsApi.Application.Models;

namespace EsportsApi.Application.Scrapers;

public class HltvScraper : ICsScraper
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public HltvScraper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<List<CsEvent>> GetLiveEvents()
    {
        var result = new List<CsEvent>();

        var httpClient = _httpClientFactory.CreateClient("Browserless");
    
        var requestBody = new
        {
            query = @"
            mutation RetrieveHltvEventsHTML {
              goto(url: ""https://www.hltv.org/events#tab-TODAY"") {
                status
              }
              html {
                html
              }
            }"
        };
    
        var response = await httpClient.PostAsync("",
            new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        );
    
        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);
        var htmlString = jsonDocument.RootElement.GetProperty("data").GetProperty("html").GetProperty("html").GetString();
        
        var parser = new HtmlParser();
        var html = await parser.ParseDocumentAsync(htmlString);

        var eventsSection = html.QuerySelector("div#TODAY.tab-content");
        var eventBoxes = eventsSection.QuerySelectorAll("a");

        foreach (var eventBox in eventBoxes)
        {
            var eventName = eventBox.QuerySelector("td.event-name-col div.text-ellipsis").TextContent.Trim();
            var eventHref = eventBox.GetAttribute("href");
            var eventId = int.Parse(eventHref.Split("/")[2]);
            
            result.Add(new CsEvent
            {
                Id = eventId,
                Name = eventName,
                Type = ""
            });
        }

        return result;
    }

    public async Task<List<CsMatch>> GetLiveMatchesFromEvent(int eventId)
    {
        var result = new List<CsMatch>();

        var httpClient = _httpClientFactory.CreateClient("Browserless");
        
        var url = $"https://www.hltv.org/events/{eventId}/matches";

        var requestBody = new
        {
            query = @"
                mutation RetrieveHltvMatchesHTML {
                  goto(url: """ + url + @""") {
                    status
                  }
                  html {
                    html
                  }
                }"
        };
        
        var response = await httpClient.PostAsync("",
            new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        );
    
        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);
        var htmlString = jsonDocument.RootElement.GetProperty("data").GetProperty("html").GetProperty("html").GetString();
        
        var parser = new HtmlParser();
        var html = await parser.ParseDocumentAsync(htmlString);
        
        var matchesSection = html.QuerySelector("div.matches-list-column");
        var matchDivs = matchesSection.QuerySelectorAll("div.match[data-livescore-match]");
        var matchUrls = matchDivs.Select(matchDiv => matchDiv.QuerySelector("a")).Select(firstAnchor => firstAnchor.GetAttribute("href")).ToList();

        foreach (var matchUrl in matchUrls)
        {
            var matchId = int.Parse(matchUrl.Split("/")[2]);
            var fullUrl = $"https://www.hltv.org{matchUrl}";
            var matchRequestBody = new
            {
                query = $@"
                mutation RetrieveHltvMatchHTML {{
                  goto(url: ""{fullUrl}"") {{
                    status
                  }}
                  waitForSelector(selector: ""div.gamelog-2d-mini-map"") {{
                    time
                  }}
                  html {{
                    html
                  }}
                }}
                "
            };
            
            var matchResponse = await httpClient.PostAsync("",
                new StringContent(JsonSerializer.Serialize(matchRequestBody), Encoding.UTF8, "application/json")
            );
    
            var matchContent = await matchResponse.Content.ReadAsStringAsync();
            var matchJsonDocument = JsonDocument.Parse(matchContent);
            var matchHtmlString = matchJsonDocument.RootElement.GetProperty("data").GetProperty("html").GetProperty("html").GetString();
            var matchHtml = await parser.ParseDocumentAsync(matchHtmlString);
            
            // SCOREBOARD DETAILS
            var scoreboard = matchHtml.QuerySelector("div.scoreboard");
            var currentMap = scoreboard.QuerySelector("span.currentRoundText").TextContent.Trim();
            var scoreBoardTeamNames = scoreboard.QuerySelectorAll("div.teamName");
            var ctTeam = scoreBoardTeamNames[0].TextContent.Trim();
            var trTeam = scoreBoardTeamNames[1].TextContent.Trim();
            var ctScore = scoreboard.QuerySelector("div.ctScore")?.TextContent.Trim();
            var trScore = scoreboard.QuerySelector("div.tScore")?.TextContent.Trim();
            //
            
            var teamsBox = matchHtml.QuerySelector("div.teamsBox");
            var teamNames = teamsBox.QuerySelectorAll("div.teamName");
            var team1Name = teamNames[0].TextContent.Trim();
            var team2Name = teamNames[1].TextContent.Trim();

            var mapsDiv = matchHtml.QuerySelectorAll("div.mapholder");
            var mapsCount = mapsDiv.Length;

            var maps = mapsDiv.Select(mapDiv =>
            {
                var teamScores = mapDiv.QuerySelectorAll("div.results-team-score");
                var team1Score = teamScores[0].TextContent.Trim();
                var team2Score = teamScores[1].TextContent.Trim();
                var mapName = mapDiv.QuerySelector("div.mapname").TextContent.Trim();
                
                if (currentMap.Contains(mapName, StringComparison.OrdinalIgnoreCase))
                {
                    
                    return new Dictionary<string, Dictionary<string, int>>
                    {
                        [mapName] = new Dictionary<string, int>
                        {
                            [team1Name] = team1Name == ctTeam ? int.Parse(ctScore) : int.Parse(trScore),
                            [team2Name] = team2Name == trTeam ? int.Parse(trScore) : int.Parse(ctScore)
                        }
                    };
                }
                
                return new Dictionary<string, Dictionary<string, int>>
                {
                    [mapName] = new Dictionary<string, int>()
                    {
                        [team1Name] = team1Score != "-" ? int.Parse(team1Score) : 0,
                        [team2Name] = team2Score != "-" ? int.Parse(team2Score) : 0
                    }
                };
            }).ToList();
            
            result.Add(new CsMatch
            {
                Id = matchId,
                Match = $"{team1Name} vs {team2Name}",
                Maps = maps,
                Type = $"Bo{mapsCount}"
            });
        }

        return result;
    }
}