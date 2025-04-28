using EsportsApi.Core.Models;

namespace EsportsApi.Core.Interfaces;

public interface ICsService
{
    List<CsEvent> GetLiveEvents();
    List<CsMatch> GetLiveMatches(int eventId);
}