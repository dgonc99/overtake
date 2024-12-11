namespace Overtake.Models.Requests;

public class CreateSimBallotRequest
{
    public required List<string> DriverPredictions { get; set; } 
    public required string Username { get; set; } 
}