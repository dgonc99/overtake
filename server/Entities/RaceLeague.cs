namespace Overtake.Entities;

public class RaceLeague
{
    public required int LeagueId { get; set; }
    public required int OwnerId { get; set; }
    public required string Name { get; set; }
    public required bool IsPublic { get; set; }
    public required DateTime CreateTime { get; set; }
}
