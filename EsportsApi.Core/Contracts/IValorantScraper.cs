using EsportsApi.Core.Models;

namespace EsportsApi.Core.Contracts;

public interface IValorantScraper
{
    public Task<List<ValorantEvent>> GetLiveEvents();
    public Task<List<ValorantMatch>> GetLiveMatches(int eventId);
}