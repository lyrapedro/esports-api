namespace EsportsApi.Application.Models;

public class LolEvent
{
    public string Event { get; set; }
    public List<LolMatch> Matches { get; set; }
}