using AngleSharp.Html.Parser;
using EsportsApi.Core.Contracts;
using EsportsApi.Core.Models;

namespace EsportsApi.Core.Scrapers;

public class VlrGgScraper : IValorantScraper
{
    public async Task<List<ValorantEvent>> GetLiveEvents()
    {
        var result = new List<ValorantEvent>();
        
        var client = new HttpClient();
        var response = await client.GetAsync("https://www.vlr.gg");
            
        var content = await response.Content.ReadAsStringAsync();
        var parser = new HtmlParser();
        var html = await parser.ParseDocumentAsync(content);
            
        var eventsColumn = html.QuerySelector("div.js-home-events");
        var liveEventsDiv = eventsColumn.QuerySelector("h1 + div.wf-module.wf-card.mod-sidebar");

        if (liveEventsDiv == null)
            return result;

        var liveEvents = liveEventsDiv.QuerySelectorAll("a");

        foreach (var eventItem in liveEvents)
        {
            var eventNameElement = eventItem.QuerySelector("div.event-item-name");
            var eventName = eventNameElement.TextContent.Trim();

            var eventUrl = eventItem.GetAttribute("href");
            eventUrl = eventUrl.Replace("/event", "", StringComparison.Ordinal);

            var eventId = int.Parse(eventUrl.Split('/')[1]);

            var eventRegionElement = eventItem.QuerySelector("div.event-item-tag");
            var eventRegion = eventRegionElement.TextContent.Trim();

            result.Add(new ValorantEvent
            {
                Id = eventId,
                Name = eventName,
                Region = eventRegion,
                Url = eventUrl
            });
        }

        return result;
    }

    public async Task<List<ValorantMatch>> GetLiveMatches(int eventId)
    {
        var result = new List<ValorantMatch>();
        
        var client = new HttpClient();
        var response = await client.GetAsync($"https://www.vlr.gg/event/matches/{eventId}");
            
        var content = await response.Content.ReadAsStringAsync();
        var parser = new HtmlParser();
        var html = await parser.ParseDocumentAsync(content);

        var liveMatchesUrl = html.QuerySelectorAll("a.wf-module-item").Where(a =>
        {
            var statusDiv = a.QuerySelector("div.ml-status");
            return statusDiv != null && statusDiv.TextContent.Trim().ToLower() == "live";
        }).Select(a => a.GetAttribute("href"));

        foreach (var matchUrl in liveMatchesUrl)
        {
            var matchPageResponse = await client.GetAsync($"https://vlr.gg{matchUrl}");
            var matchPageContent = await matchPageResponse.Content.ReadAsStringAsync();

            var matchPageHtml = await parser.ParseDocumentAsync(matchPageContent);

            var matchId = int.Parse(matchUrl.Split("/")[1]);
            var teamNames = matchPageHtml.QuerySelectorAll(".team-name").Select(n => n.TextContent.Trim()).ToList();

            var team1Name = teamNames[0];
            var team2Name = teamNames[1];

            var matchTypeDivs = matchPageHtml.QuerySelectorAll("div.match-header-vs-note");
            var matchType = matchTypeDivs[1].TextContent.Trim();

            var mapScore = matchPageHtml.QuerySelectorAll(".match-header-vs-score span");
            var team1MapScore = mapScore[1].TextContent.Trim();
            var team2MapScore = mapScore[3].TextContent.Trim();

            var mapDivs = matchPageHtml.QuerySelectorAll("div.vm-stats-game[data-game-id]")
                .Where(md => md.GetAttribute("data-game-id") != "all");

            var maps = mapDivs.Select(div =>
            {
                var mapNameElement = div.QuerySelector(".map span");
                var mapName = mapNameElement.ChildNodes[0].TextContent.Trim();

                var teamDivs = div.QuerySelectorAll(".team");
                var team1Score = teamDivs[0]?.QuerySelector(".score")?.TextContent.Trim() ?? "-";
                var team2Score = teamDivs[1]?.QuerySelector(".score")?.TextContent.Trim() ?? "-";
                Console.WriteLine(team1Score);
                Console.WriteLine(team2Score);
                
                return new Dictionary<string, Dictionary<string, int>>
                {
                    [mapName] = new Dictionary<string, int>
                    {
                        [teamNames[0]] = int.Parse(team1Score),
                        [teamNames[1]] = int.Parse(team2Score)
                    }
                };
            }).ToList();
            
            result.Add(new ValorantMatch
            {
                Id = matchId,
                Match = $"{team1Name} vs {team2Name}",
                Maps = maps,
                Type = matchType,
                MapScore = new Dictionary<string, int>
                {
                    [teamNames[0]] = int.Parse(team1MapScore),
                    [teamNames[1]] = int.Parse(team2MapScore)
                }
            });
        }
        
        return result;
    }
}