namespace Overtake.Entities;

public class Race
{
    public required int RaceId { get; set; }
    public required int TrackId { get; set; }
    public required DateTime StartTime { get; set; }
}
