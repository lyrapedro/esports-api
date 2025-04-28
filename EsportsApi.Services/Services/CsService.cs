using EsportsApi.Core.Interfaces;
using EsportsApi.Core.Models;
using EsportsApi.Core.Scrapers;

namespace EsportsApi.Services;

public class CsService : ICsService
{
    private readonly HltvScraper _scraper;
    
    public CsService(HltvScraper scraper)
    {
        _scraper = scraper;
    }
    
    public List<CsEvent> GetLiveEvents()
    {
        throw new NotImplementedException();
    }

    public List<CsMatch> GetLiveMatches(int eventId)
    {
        throw new NotImplementedException();
    }
}