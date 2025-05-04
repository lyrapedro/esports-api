using EsportsApi.Application.Models;

namespace EsportsApi.Application.Contracts;

public interface ILolScraper
{
    public Task<List<LolMatch>> GetLiveMatches();
}