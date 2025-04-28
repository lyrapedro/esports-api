using EsportsApi.Core.Contracts;
using EsportsApi.Core.Models;

namespace EsportsApi.Core.Scrapers;

public class HltvScraper : ICsScraper
{
    public List<CsEvent> GetLiveEvents()
    {
        throw new NotImplementedException();
    }

    public List<CsMatch> GetLiveMatches(int eventId)
    {
        throw new NotImplementedException();
    }
}