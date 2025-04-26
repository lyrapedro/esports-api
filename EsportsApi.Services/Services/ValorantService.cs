using EsportsApi.Services.Models;
using EsportsApi.Services.Interfaces;
using EsportsApi.Core.Scrapers.VlrGG;
using HtmlAgilityPack;

namespace EsportsApi.Services;

public class ValorantService : IValorantService
{
    private VlrGGScraper _scraper;
    
    public ValorantService()
    {
        _scraper = new VlrGGScraper();
    }
   
    public List<ValorantEvent> GetLiveEvents()
    {
        var result = new List<ValorantEvent>();

        var liveEvents = _scraper.GetLiveEventsNodes();
        
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
        
        var liveMatches = _scraper.GetLiveMatchesNodes(eventUrl);
        
        foreach (var match in liveMatches)
        {
            // Extrair nomes dos times
            var teamsDiv = match.SelectSingleNode(".//div[contains(@class, 'match-item-vs')]");
            var teamsNames = teamsDiv.SelectNodes(".//div[contains(@class, 'match-item-vs-team-name')]");
            
            var team1Name = teamsNames[0].SelectSingleNode(".//div[contains(@class, 'text-of')]").InnerText.Trim();
            var team2Name = teamsNames[1].SelectSingleNode(".//div[contains(@class, 'text-of')]").InnerText.Trim();
            
            // Obter página do match
            var matchUrl = match.GetAttributeValue("href", "");

            var bsMatch = _scraper.GetLiveMatchPage(matchUrl);
            
            // Extrair tipo do match
            var matchType = bsMatch.DocumentNode.SelectNodes("//div[contains(@class, 'match-header-vs-note')]")[1].InnerText.Trim();
            
            // Extrair placar do match
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
                    .FirstOrDefault(div => div.GetClasses().Contains("map"))?    // busca a div com classe 'map'
                    .Descendants("span")
                    .FirstOrDefault()?                                           // primeiro span dentro dessa div
                    .ChildNodes
                    .FirstOrDefault(node => node.NodeType == HtmlNodeType.Text)?  // primeiro nó de texto direto
                    .InnerText
                    .Trim();
                
                // Pega todas as divs que têm a classe 'team'
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
}