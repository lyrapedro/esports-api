using EsportsApi.Application.Models;

namespace EsportsApi.Application.Contracts;

public interface IValorantScraper
{
    public Task<List<ValorantEvent>> GetLiveEvents();
    public Task<List<ValorantMatch>> GetLiveMatches(int eventId);
}