namespace Overtake.Entities;

public class Track
{
    public required int TrackId { get; set; }
    public required string Name { get; set; }
    public required string Location { get; set; }
    public required double Distance { get; set; }
    public required int Turns { get; set; }
}

