namespace Overtake.Entities;

public class LeagueInvite
{
    public required int InviteId { get; set; }
    public required int LeagueId { get; set; }
    public required int InviteeId { get; set; }
    public required DateTime RequestTime { get; set; }
    public required int Status { get; set; }
}
