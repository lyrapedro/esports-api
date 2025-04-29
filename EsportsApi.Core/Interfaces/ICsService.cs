using EsportsApi.Core.Models;

namespace EsportsApi.Core.Interfaces;

public interface ICsService
{
    Task<List<CsEvent>> GetLiveEvents();
    Task<List<CsMatch>> GetLiveMatches(int eventId);
}