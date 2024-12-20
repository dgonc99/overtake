namespace Overtake.Models.Requests;

public class CreateBallotRequest
{
    public required List<string> DriverPredictions { get; set; } // List of 10 driver IDs representing the user's predictions
    public int? TotalScore { get; set; } // Total Score of Ballot
    public int? LeagueId { get; set; } // Corresponding league_id of user's selected league
}