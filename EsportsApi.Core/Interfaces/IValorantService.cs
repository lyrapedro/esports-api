using EsportsApi.Core.Models;

namespace EsportsApi.Core.Interfaces;

public interface IValorantService
{
    List<ValorantEvent> GetLiveEvents();
    List<ValorantMatch> GetLiveMatches(string eventUrl);
}