namespace Overtake.Entities;

public class Driver
{
    public required int DriverId { get; set; }
    public required int DriverNumber { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required int Age { get; set; }
    public required string Nationality { get; set; }
    public required double Height { get; set; }
    public required int TeamId { get; set; }
}
