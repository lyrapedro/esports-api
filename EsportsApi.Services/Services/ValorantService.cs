using EsportsApi.Core.Interfaces;
using EsportsApi.Core.Models;
using EsportsApi.Core.Scrapers;

namespace EsportsApi.Services;

public class ValorantService : IValorantService
{
    private readonly VlrGgScraper _scraper;
    
    public ValorantService(VlrGgScraper scraper)
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