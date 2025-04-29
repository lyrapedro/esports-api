using EsportsApi.Core.Contracts;
using EsportsApi.Core.Interfaces;
using EsportsApi.Core.Models;

namespace EsportsApi.Services;

public class CsService : ICsService
{
    private readonly ICsScraper _scraper;
    
    public CsService(ICsScraper scraper)
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