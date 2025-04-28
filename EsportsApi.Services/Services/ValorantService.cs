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
   
    public async Task<List<ValorantEvent>> GetLiveEvents()
    {
        var liveEvents = await _scraper.GetLiveEvents();

        return liveEvents;
    }

    public async Task<List<ValorantMatch>> GetLiveMatches(int eventId)
    {
       
        var liveMatches = await _scraper.GetLiveMatches(eventId);
        
        return liveMatches;
    }
}