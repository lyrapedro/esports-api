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
    
    public async Task<List<CsEvent>> GetLiveEvents()
    {
        return await _scraper.GetLiveEvents();
    }

    public async Task<List<CsMatch>> GetLiveMatches(int eventId)
    {
        throw new NotImplementedException();
    }
}