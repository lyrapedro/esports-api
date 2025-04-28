using EsportsApi.Core.Contracts;
using EsportsApi.Services.Helpers.Extensions;
using EsportsApi.Core.Models;
using HtmlAgilityPack;

namespace EsportsApi.Core.Scrapers.VlrGG;

public class VlrGGScraper : IValorantScraper
{
    public List<ValorantEvent> GetLiveEvents()
    {
        var result = new List<ValorantEvent>();
        
        var document = new HtmlWeb().Load("https://www.vlr.gg");
        
        var liveEventsH1 = document.DocumentNode.Descendants("h1")
            .FirstOrDefault(h1 => 
                h1.GetAttributeValue("class", "").Contains("wf-label") && 
                h1.GetAttributeValue("class", "").Contains("mod-sidebar") &&
                h1.InnerText.ToLower().Trim().Contains("live events"));
        
        var targetDiv = liveEventsH1.NextSiblings()
            .FirstOrDefault(node => 
                node.Name == "div" && 
                node.GetAttributeValue("class", "").Split(' ')
                    .Intersect(new[] { "wf-module", "wf-card", "mod-sidebar" })
                    .Count() == 3);
        
        var liveEvents = targetDiv.Descendants("a")
            .Where(a => a.GetAttributeValue("class", "").Split(' ')
                .Intersect(new[] { "wf-module-item", "event-item" })
                .Count() == 2)
            .ToList();
        
        foreach (var eventItem in liveEvents)
        {
            var eventName = eventItem.SelectSingleNode(".//div[contains(@class, 'event-item-name')]")?
                .InnerText.Trim();
            
            var eventUrl = eventItem.GetAttributeValue("href", "");
            eventUrl = eventUrl.Replace("/event", "", StringComparison.Ordinal);
            
            var eventId = int.Parse(eventUrl.Split('/')[1]);
            
            var eventRegion = eventItem.SelectSingleNode(".//div[contains(@class, 'event-item-tag')]")?
                .InnerText.Trim();
            
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

    public List<ValorantMatch> GetLiveMatches(string eventUrl)
    {
        var result = new List<ValorantMatch>();
        
        var decodedUri = Uri.UnescapeDataString(eventUrl);
        var url = $"https://www.vlr.gg/event/matches{decodedUri}";
        
        var document = new HtmlWeb().Load(url);
        
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

            var bsMatch = GetLiveMatchPage(matchUrl);
            
            var matchType = bsMatch.DocumentNode.SelectNodes("//div[contains(@class, 'match-header-vs-note')]")[1].InnerText.Trim();
            
            var mapScore = bsMatch.DocumentNode.SelectNodes("//div[contains(@class, 'match-header-vs-score')]")[1]
                .SelectNodes(".//span");
            var team1MapScore = mapScore[0].InnerText.Trim();
            var team2MapScore = mapScore[2].InnerText.Trim(); // 2 porque o índice 1 é o separador (:)
            
            // Extrair mapas
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
                Name = $"{team1Name} vs {team2Name}",
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

    private HtmlDocument GetLiveMatchPage(string matchUrl)
    {
        return new HtmlWeb().Load($"https://www.vlr.gg{matchUrl}");
    }
}