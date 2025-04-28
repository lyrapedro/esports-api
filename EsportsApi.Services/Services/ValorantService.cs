using EsportsApi.Core.Interfaces;
using EsportsApi.Core.Models;
using EsportsApi.Core.Scrapers.VlrGG;

namespace EsportsApi.Services;

public class ValorantService : IValorantService
{
    private readonly VlrGGScraper _scraper;
    
    public ValorantService(VlrGGScraper scraper)
    {
        _scraper = scraper;
    }
   
    public List<ValorantEvent> GetLiveEvents()
    {
        var liveEvents = _scraper.GetLiveEvents();

        return liveEvents;
    }

    public List<ValorantMatch> GetLiveMatches(string eventUrl)
    {
       
        var liveMatches = _scraper.GetLiveMatches(eventUrl);
        
        return liveMatches;
    }
}