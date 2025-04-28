using EsportsApi.Core.Models;

namespace EsportsApi.Core.Contracts;

public interface ICsScraper
{
    public List<CsEvent> GetLiveEvents();
    public List<CsMatch> GetLiveMatches(int eventId);
}