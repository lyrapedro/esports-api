using EsportsApi.Core.Models;

namespace EsportsApi.Core.Contracts;

public interface IValorantScraper
{
    public List<ValorantEvent> GetLiveEvents();
    public List<ValorantMatch> GetLiveMatches(string eventUrl);
}