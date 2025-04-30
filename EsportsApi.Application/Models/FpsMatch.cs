namespace EsportsApi.Application.Models;

public class FpsMatch
{
    public int Id { get; set; }
    public string Match { get; set; }
    public string Type { get; set; }
    public Dictionary<string, int> MapScore { get; set; }
    public List<Dictionary<string, Dictionary<string, int>>> Maps { get; set; }
}