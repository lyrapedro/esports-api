using EsportsApi.Core.Models;

namespace EsportsApi.Core.Contracts;

public interface ICsScraper
{
    public Task<List<CsEvent>> GetLiveEvents();
    public Task<List<CsMatch>> GetLiveMatches(int eventId);
}