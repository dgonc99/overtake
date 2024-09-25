namespace Overtake.Entities;

public class RaceLeagueMembership
{
    public required int LeagueId { get; set; }
    public required int UserId { get; set; }
    public required DateTime JoinTime { get; set; }
}
