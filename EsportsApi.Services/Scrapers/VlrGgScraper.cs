using System.Diagnostics;
using EsportsApi.Core.Contracts;
using EsportsApi.Services.Helpers.Extensions;
using EsportsApi.Core.Models;
using HtmlAgilityPack;
using PuppeteerSharp;

namespace EsportsApi.Core.Scrapers;

public class VlrGgScraper : IValorantScraper
{
    private readonly IBrowser _browser;

    public VlrGgScraper(IBrowser browser)
    {
        _browser = browser;
    }

    public async Task<List<ValorantEvent>> GetLiveEvents()
    {
        var result = new List<ValorantEvent>();
        var page = await _browser.NewPageAsync();
        
        try
        {
            await page.GoToAsync("https://www.vlr.gg");

            var eventsColumn = await page.WaitForSelectorAsync("div.js-home-events");

            var liveEventsDiv = await eventsColumn.QuerySelectorAsync("h1 + div.wf-module.wf-card.mod-sidebar");

            if (liveEventsDiv == null)
                return result;

            var liveEvents = await liveEventsDiv.QuerySelectorAllAsync("a");

            foreach (var eventItem in liveEvents)
            {
                var eventNameElement = await eventItem.QuerySelectorAsync("div.event-item-name");
                var eventName = eventNameElement != null
                    ? (await page.EvaluateFunctionAsync<string>("el => el.textContent.trim()", eventNameElement))
                    : null;

                var eventUrl = await page.EvaluateFunctionAsync<string>("el => el.getAttribute('href')", eventItem) ??
                               "";
                eventUrl = eventUrl.Replace("/event", "", StringComparison.Ordinal);

                var eventId = int.Parse(eventUrl.Split('/')[1]);

                var eventRegionElement = await eventItem.QuerySelectorAsync("div.event-item-tag");
                var eventRegion = eventRegionElement != null
                    ? (await page.EvaluateFunctionAsync<string>("el => el.textContent.trim()", eventRegionElement))
                    : null;

                result.Add(new ValorantEvent
                {
                    Id = eventId,
                    Name = eventName,
                    Region = eventRegion,
                    Url = eventUrl
                });
            }
        }
        finally
        {
            await page.CloseAsync();
        }

        return result;
    }

    public async Task<List<ValorantMatch>> GetLiveMatches(int eventId)
    {
        var result = new List<ValorantMatch>();
        
        var url = $"https://www.vlr.gg/event/matches/{eventId}";
        
        var document = await new HtmlWeb().LoadFromWebAsync(url);
        
        var liveMatches = document.DocumentNode.Descendants("a")
            .Where(a => a.GetAttributeValue("class", "").Contains("wf-module-item"))
            .Where(a => 
            {
                var mlStatus = a.Descendants("div")
                    .FirstOrDefault(d => d.GetAttributeValue("class", "").Contains("ml-status"));
                
                return mlStatus != null && mlStatus.InnerText.Trim() == "LIVE";
            })
            .ToList();

        foreach (var match in liveMatches)
        {
            var teamsDiv = match.SelectSingleNode(".//div[contains(@class, 'match-item-vs')]");
            var teamsNames = teamsDiv.SelectNodes(".//div[contains(@class, 'match-item-vs-team-name')]");
            
            var team1Name = teamsNames[0].SelectSingleNode(".//div[contains(@class, 'text-of')]").InnerText.Trim();
            var team2Name = teamsNames[1].SelectSingleNode(".//div[contains(@class, 'text-of')]").InnerText.Trim();
            
            var matchUrl = match.GetAttributeValue("href", "");

            var bsMatch = await GetLiveMatchPage(matchUrl);
            
            var matchType = bsMatch.DocumentNode.SelectNodes("//div[contains(@class, 'match-header-vs-note')]")[1].InnerText.Trim();
            
            var mapScore = bsMatch.DocumentNode.SelectNodes("//div[contains(@class, 'match-header-vs-score')]")[1]
                .SelectNodes(".//span");
            var team1MapScore = mapScore[0].InnerText.Trim();
            var team2MapScore = mapScore[2].InnerText.Trim(); // 2 porque o índice 1 é o separador (:)
            
            var mapsContainers = bsMatch.DocumentNode
                .Descendants("div")
                .Where(div => 
                    div.GetClasses().Contains("vm-stats-game") && 
                    div.Attributes.Contains("data-game-id"))
                .ToList();
            var mapDivs = new List<HtmlNode>();
            
            foreach (var div in mapsContainers)
            {
                var gameId = div.GetAttributeValue("data-game-id", "");
                if (gameId != "all")
                {
                    mapDivs.Add(div);
                }
            }
            
            var maps = new List<Dictionary<string, Dictionary<string, int>>>();
            
            foreach (var mapDiv in mapDivs)
            {
                var mapName = mapDiv
                    .Descendants("div")
                    .FirstOrDefault(div => div.GetClasses().Contains("map"))?
                    .Descendants("span")
                    .FirstOrDefault()?
                    .ChildNodes
                    .FirstOrDefault(node => node.NodeType == HtmlNodeType.Text)?
                    .InnerText
                    .Trim();
                
                var teams = mapDiv
                    .Descendants("div")
                    .Where(div => div.GetClasses().Contains("team"))
                    .ToList();

                var team1Score = teams[0]
                    .Descendants()
                    .FirstOrDefault(node => node.GetClasses().Contains("score"))?
                    .InnerText
                    .Trim();

                var team2Score = teams[1]
                    .Descendants()
                    .FirstOrDefault(node => node.GetClasses().Contains("score"))?
                    .InnerText
                    .Trim();

                
                maps.Add(new Dictionary<string, Dictionary<string, int>>
                {
                    [mapName] = new Dictionary<string, int>
                    {
                        [team1Name] = int.Parse(team1Score),
                        [team2Name] = int.Parse(team2Score)
                    }
                });
            }
            
            result.Add(new ValorantMatch
            {
                Match = $"{team1Name} vs {team2Name}",
                Type = matchType,
                MapScore = new Dictionary<string, int>
                {
                    [team1Name] = int.Parse(team1MapScore),
                    [team2Name] = int.Parse(team2MapScore)
                },
                Maps = maps
            });
        }

        return result;
    }

    private async Task<HtmlDocument> GetLiveMatchPage(string matchUrl)
    {
        return await new HtmlWeb().LoadFromWebAsync($"https://www.vlr.gg{matchUrl}");
    }
}