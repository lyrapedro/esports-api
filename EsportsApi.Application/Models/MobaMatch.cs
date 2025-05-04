namespace EsportsApi.Application.Models;

public class MobaMatch
{
    public string Match { get; set; }
    public string Type { get; set; }
    public Dictionary<string, int> Score { get; set; }
}