using EsportsApi.Core.Helpers.Extensions;
using HtmlAgilityPack;

namespace EsportsApi.Core.Scrapers.VlrGG;

public class VlrGGScraper
{
    private HtmlWeb _web;
    
    public VlrGGScraper()
    {
        _web = new HtmlWeb();
    }

    public List<HtmlNode> GetLiveEventsNodes()
    {
        var document = _web.Load("https://www.vlr.gg");
        
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

        return liveEvents;
    }

    public List<HtmlNode> GetLiveMatchesNodes(string eventUrl)
    {
        var decodedUri = Uri.UnescapeDataString(eventUrl);
        var url = $"https://www.vlr.gg/event/matches{decodedUri}";
        
        var document = _web.Load(url);
        
        var liveMatches = document.DocumentNode.Descendants("a")
            .Where(a => a.GetAttributeValue("class", "").Contains("wf-module-item"))
            .Where(a => 
            {
                var mlStatus = a.Descendants("div")
                    .FirstOrDefault(d => d.GetAttributeValue("class", "").Contains("ml-status"));
                
                return mlStatus != null && mlStatus.InnerText.Trim() == "LIVE";
            })
            .ToList();

        return liveMatches;
    }

    public HtmlDocument GetLiveMatchPage(string matchUrl)
    {
        return _web.Load($"https://www.vlr.gg{matchUrl}");
    }
}