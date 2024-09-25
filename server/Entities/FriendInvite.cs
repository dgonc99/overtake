namespace Overtake.Entities;

public class FriendInvite
{
    public required int InviteId { get; set; }
    public required int InitiatorId { get; set; }
    public required int InviteeId { get; set; }
    public required string Message { get; set; }
    public required DateTime RequestTime { get; set; }
    public required int Status { get; set; }
}
