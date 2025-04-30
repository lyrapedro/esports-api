using EsportsApi.Application.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EsportsApi.API.Controllers;

[Route("api/cs")]
public class CsController : Controller
{
    private readonly ICsScraper _scraper;

    public CsController(ICsScraper scraper)
    {
        _scraper = scraper;
    }
    
    [HttpGet("events")]
    public async Task<IActionResult> GetLiveEvents()
    {
        try
        {
            return Ok(await _scraper.GetLiveEvents());
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpGet("event/{eventId}/matches")]
    public async Task<IActionResult> GetLiveMatchesFromEvent(int eventId)
    {
        try
        {
            return Ok(await _scraper.GetLiveMatchesFromEvent(eventId));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}