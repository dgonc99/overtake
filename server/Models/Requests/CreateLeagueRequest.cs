namespace Overtake.Models.Requests;

public class CreateLeagueRequest
{
    public required string Name { get; set; }
    public required bool IsPublic { get; set; }
}
