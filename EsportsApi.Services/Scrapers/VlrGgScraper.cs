using System.Text.Json;
using EsportsApi.Core.Contracts;
using EsportsApi.Core.Models;
using EsportsApi.Services.DTO;
using PuppeteerSharp;

namespace EsportsApi.Core.Scrapers;

public class VlrGgScraper : IValorantScraper
{
    private readonly IBrowser _browser;

    public VlrGgScraper(IBrowser browser)
    {
        _browser = browser;
    }

    public async Task<List<ValorantEvent>> GetLiveEvents()
    {
        var result = new List<ValorantEvent>();
        var page = await _browser.NewPageAsync();
        
        try
        {
            await page.GoToAsync("https://www.vlr.gg");

            var eventsColumn = await page.WaitForSelectorAsync("div.js-home-events");

            var liveEventsDiv = await eventsColumn.QuerySelectorAsync("h1 + div.wf-module.wf-card.mod-sidebar");

            if (liveEventsDiv == null)
                return result;

            var liveEvents = await liveEventsDiv.QuerySelectorAllAsync("a");

            foreach (var eventItem in liveEvents)
            {
                var eventNameElement = await eventItem.QuerySelectorAsync("div.event-item-name");
                var eventName = eventNameElement != null
                    ? (await page.EvaluateFunctionAsync<string>("el => el.textContent.trim()", eventNameElement))
                    : null;

                var eventUrl = await page.EvaluateFunctionAsync<string>("el => el.getAttribute('href')", eventItem) ??
                               "";
                eventUrl = eventUrl.Replace("/event", "", StringComparison.Ordinal);

                var eventId = int.Parse(eventUrl.Split('/')[1]);

                var eventRegionElement = await eventItem.QuerySelectorAsync("div.event-item-tag");
                var eventRegion = eventRegionElement != null
                    ? (await page.EvaluateFunctionAsync<string>("el => el.textContent.trim()", eventRegionElement))
                    : null;

                result.Add(new ValorantEvent
                {
                    Id = eventId,
                    Name = eventName,
                    Region = eventRegion,
                    Url = eventUrl
                });
            }
        }
        finally
        {
            await page.CloseAsync();
        }

        return result;
    }

    public async Task<List<ValorantMatch>> GetLiveMatches(int eventId)
    {
        var result = new List<ValorantMatch>();

        var page = await _browser.NewPageAsync();

        try
        {
            await page.GoToAsync($"https://www.vlr.gg/event/matches/{eventId}");

            var liveMatchesUrl = await page.EvaluateFunctionAsync<List<string>>(@"() => {
                return Array.from(document.querySelectorAll('a.wf-module-item'))
                    .filter(a => {
                        const statusDiv = a.querySelector('div.ml-status');
                        return statusDiv && statusDiv.innerText.trim() === 'LIVE';
                    })
                    .map(a => a.getAttribute('href'));
            }");

            foreach (var matchUrl in liveMatchesUrl)
            {
                await page.GoToAsync($"https://vlr.gg{matchUrl}");

                var matchId = int.Parse(matchUrl.Split("/")[1]);

                var matchDataJson = await page.EvaluateFunctionAsync<string>(@"() => {
                    const teamNames = Array.from(document.querySelectorAll('.team-name'))
                        .map(el => el.innerText.trim());

                    const matchTypeDivs = document.querySelectorAll('div.match-header-vs-note');
                    const matchType = matchTypeDivs.length > 1 ? matchTypeDivs[1].innerText.trim() : '';

                    const mapScore = document.querySelectorAll('.match-header-vs-score span');
                    const team1MapScore = mapScore[1].innerText.trim();
                    const team2MapScore = mapScore[3].innerText.trim();

                    const mapDivs = Array.from(document.querySelectorAll('div.vm-stats-game[data-game-id]'))
                        .filter(div => div.getAttribute('data-game-id') !== 'all');

                    const maps = mapDivs.map(div => {
                        const mapNameElement = div.querySelector('.map span');
                        const mapName = mapNameElement ? mapNameElement.childNodes[0].textContent.trim() : '';

                        const teamDivs = div.querySelectorAll('.team');
                        const team1Score = teamDivs[0]?.querySelector('.score')?.innerText.trim() || '0';
                        const team2Score = teamDivs[1]?.querySelector('.score')?.innerText.trim() || '0';

                        return {
                            [mapName]: {
                                [teamNames[0]]: parseInt(team1Score),
                                [teamNames[1]]: parseInt(team2Score)
                            }
                        };
                    });

                    return JSON.stringify({
                        team1Name: teamNames[0],
                        team2Name: teamNames[1],
                        matchType,
                        team1MapScore,
                        team2MapScore,
                        maps
                    });
                }");

                var matchData = JsonSerializer.Deserialize<MatchData>(matchDataJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                result.Add(new ValorantMatch
                {
                    Id = matchId,
                    Match = $"{matchData.Team1Name} vs {matchData.Team2Name}",
                    Type = matchData.MatchType,
                    MapScore = new Dictionary<string, int>
                    {
                        [matchData.Team1Name] = int.Parse(matchData.Team1MapScore),
                        [matchData.Team2Name] = int.Parse(matchData.Team2MapScore)
                    },
                    Maps = matchData.Maps
                });
            }

            return result;
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}