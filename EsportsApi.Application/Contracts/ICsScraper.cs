using EsportsApi.Application.Models;

namespace EsportsApi.Application.Contracts;

public interface ICsScraper
{
    public Task<List<CsEvent>> GetLiveEvents();
    public Task<List<CsMatch>> GetLiveMatchesFromEvent(int eventId);
}