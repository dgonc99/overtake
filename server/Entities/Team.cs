namespace Overtake.Entities;

public class Team
{
    public required int TeamId { get; set; }
    public required string Name { get; set; }
    public required string Nationality { get; set; }
    public required DateTime CreateTime { get; set; }
}
