namespace EsportsApi.Services.Models;

public class ValorantMatch
{
    public string Name { get; set; }
    public string Type { get; set; }
    public Dictionary<string, int> MapScore { get; set; }
    public List<Dictionary<string, Dictionary<string, int>>> Maps { get; set; }
}