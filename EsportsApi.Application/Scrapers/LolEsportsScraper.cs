using AngleSharp.Html.Parser;
using EsportsApi.Application.Contracts;
using EsportsApi.Application.Models;

namespace EsportsApi.Application.Scrapers;

public class LolEsportsScraper : ILolScraper
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LolEsportsScraper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<List<LolMatch>> GetLiveMatches()
    {
        var result = new List<LolMatch>();
        
        var client = _httpClientFactory.CreateClient("LolEsportsClient");
        var response = await client.GetAsync("");
        
        var content = await response.Content.ReadAsStringAsync();
        var parser = new HtmlParser();
        var html = await parser.ParseDocumentAsync(content);

        var currentEventName = html.QuerySelector("span.c_home.seasonOverview.accentText").TextContent.Trim();

        return result;
    }
}