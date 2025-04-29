using System.Text;
using System.Text.Json;
using AngleSharp.Html.Parser;
using EsportsApi.Core.Contracts;
using EsportsApi.Core.Models;

namespace EsportsApi.Core.Scrapers;

public class HltvScraper : ICsScraper
{
    public async Task<List<CsEvent>> GetLiveEvents()
    {
        var result = new List<CsEvent>();
        
        var endpoint = "https://production-sfo.browserless.io/chromium/bql";
        var token = "S7onTb80xNTt4w5ae08c992979cb077821170a1e88";
        
        using var httpClient = new HttpClient();
    
        var requestBody = new
        {
            query = @"
            mutation RetrieveHltvEventsHTML {
              goto(url: ""https://www.hltv.org/events#tab-TODAY"") {
                status
              }
              html {
                html
              }
            }"
        };
    
        var response = await httpClient.PostAsync(
            $"{endpoint}?token={token}",
            new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
        );
    
        var content = await response.Content.ReadAsStringAsync();

        var jsonDocument = JsonDocument.Parse(content);
        
        var htmlString = jsonDocument.RootElement.GetProperty("data").GetProperty("html").GetProperty("html").GetString();
        
        var parser = new HtmlParser();
        var html = await parser.ParseDocumentAsync(htmlString);

        var eventsSection = html.QuerySelector("div#TODAY.tab-content");
        var eventBoxes = eventsSection.QuerySelectorAll("a");

        foreach (var eventBox in eventBoxes)
        {
            var eventName = eventBox.QuerySelector("td.event-name-col div.text-ellipsis").TextContent.Trim();
            var eventHref = eventBox.GetAttribute("href");
            var eventId = int.Parse(eventHref.Split("/")[2]);
            
            result.Add(new CsEvent
            {
                Id = eventId,
                Name = eventName,
                Type = ""
            });
        }

        return result;
    }

    public async Task<List<CsMatch>> GetLiveMatches(int eventId)
    {
        throw new NotImplementedException();
    }
}