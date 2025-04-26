using EsportsApi.Services.Models;

namespace EsportsApi.Services.Interfaces;

public interface IValorantService
{
    public List<ValorantEvent> GetLiveEvents();
    public List<ValorantMatch> GetLiveMatches(string eventUrl);
}