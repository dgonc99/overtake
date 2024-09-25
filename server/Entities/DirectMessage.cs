namespace Overtake.Entities;

public class DirectMessage
{
    public required int MessageId { get; set; }
    public required int SenderId { get; set; }
    public required int ReceiverId { get; set; }
    public required string Content { get; set; }
    public required DateTime SendTime { get; set; }
    public DateTime ReadTime { get; set; }
}
