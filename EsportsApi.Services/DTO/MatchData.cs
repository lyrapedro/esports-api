namespace EsportsApi.Services.DTO;

public class MatchData
{
    public int MatchId { get; set; }
    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
    public string MatchType { get; set; }
    public string Team1MapScore { get; set; }
    public string Team2MapScore { get; set; }
    public List<Dictionary<string, Dictionary<string, int>>> Maps { get; set; }
}