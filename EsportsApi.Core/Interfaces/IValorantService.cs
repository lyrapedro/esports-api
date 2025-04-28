using EsportsApi.Core.Models;

namespace EsportsApi.Core.Interfaces;

public interface IValorantService
{
    Task<List<ValorantEvent>> GetLiveEvents();
    Task<List<ValorantMatch>> GetLiveMatches(int eventId);
}