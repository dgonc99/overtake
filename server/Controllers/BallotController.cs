using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Overtake.Interfaces;
using Overtake.Models;
using Overtake.Models.Requests;
using Overtake.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/ballot")]
[ApiController]
public class BallotController : ControllerBase
{
    private readonly IDatabase _database;
    private readonly ILogger<BallotController> _logger;

    public BallotController(IDatabase database, ILogger<BallotController> logger)
    {
        _database = database;
        _logger = logger;
    }

    [HttpPost]
    [Route("create")]
    [Produces("application/json")]
    public async Task<ActionResult<int>> CreateAsync([FromBody] CreateBallotRequest request)
    {

        // Validate request
        if (request.DriverPredictions == null || request.DriverPredictions.Count != 10)
        {
            return new BadRequestResult();
        }

        // Validate leagueId
        if (!request.LeagueId.HasValue)
        {
            return new BadRequestObjectResult("LeagueId is required");
        }

        // Get current user ID
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        int leagueId = request.LeagueId.Value;

        /*
        // Get the raceId of the next race
        int? nextRaceId = await _database.GetNextRaceId();
        if (!nextRaceId.HasValue)
        {
            return new BadRequestObjectResult("No upcoming race found.");
        }

        int raceId = nextRaceId.Value;
        */

        // Hardcode to assign raceId to Abu Dhabi 2024
        int raceId = 24;

        // Create a list of DriverPrediction objects
        var driverPredictions = request.DriverPredictions.Select((name, index) => new DriverPrediction
        {
            DriverName = name,
            Position = index + 1 // Assuming the positions are 1-based
        }).ToList();

        // Extract totalScore from request
        int? totalScore = request.TotalScore;

        // Call the InsertBallotAsync method
        int newBallotId = await _database.InsertBallotAsync(userId, leagueId, raceId, driverPredictions, totalScore);

        return new OkObjectResult(newBallotId);
    }

    [HttpGet]
    [Route("populate")]
    [Produces("application/json")]
    public async Task<ActionResult<BallotContent[]>> PopulateAsync(int leagueId)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        int? ballotId = await _database.GetBallotByUserIdAndLeagueIdAsync(userId, leagueId);

        if (ballotId == null)
        {
            return NotFound();
        }

        BallotContent[] ballot = await _database.GetBallotContentAsync(ballotId.Value);

        if (ballot == null || ballot.Length == 0)
        {
            return Ok(new BallotContent[0]);
        }

        return new OkObjectResult(ballot);
    }

    [HttpPut]
    [Route("update")]
    [Produces("application/json")]
    public async Task<ActionResult> UpdateAsync([FromBody] CreateBallotRequest request)
    {
        // Validate request
        if (request.DriverPredictions == null || request.DriverPredictions.Count != 10)
        {
            return BadRequest("All 10 driver positions must be provided.");
        }

        // Validate leagueId
        if (!request.LeagueId.HasValue)
        {
            return BadRequest("LeagueId is required.");
        }

        // Get current user ID
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        int leagueId = request.LeagueId.Value;

        // Get the raceId of the next race
        int? nextRaceId = await _database.GetNextRaceId();
        if (!nextRaceId.HasValue)
        {
            return new BadRequestObjectResult("No upcoming race found.");
        }

        int raceId = nextRaceId.Value;

        // Create a list of DriverPrediction objects
        var driverPredictions = request.DriverPredictions.Select((name, index) => new DriverPrediction
        {
            DriverName = name,
            Position = index + 1
        }).ToList();

        // Call the database update method
        bool updateSuccess = await _database.UpdateBallotAsync(userId, leagueId, raceId, driverPredictions);

        if (!updateSuccess)
        {
            return NotFound("No ballot found to update.");
        }

        return Ok("Ballot updated successfully.");
    }

    [HttpPut]
    [Route("updateScore")]
    [Produces("application/json")]
    public async Task<ActionResult> UpdateScoreAsync([FromBody] UpdateScoreRequest request)
    {
        // Extract userId from the claims in the HTTP context
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // Call the updated UpdateBallotScoreAsync with userId, leagueId, and raceId
        bool result = await _database.UpdateBallotScoreAsync(userId, request.LeagueId, request.RaceId, request.Score);

        if (result)
        {
            return Ok("Score updated successfully.");
        }
        else
        {
            return NotFound("Ballot not found or could not update score.");
        }
    }

    [HttpGet]
    [Route("getBallotId")]
    [Produces("application/json")]
    public async Task<ActionResult<int?>> GetBallotIdAsync(int leagueId)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        int? ballotId = await _database.GetBallotByUserIdAndLeagueIdAsync(userId, leagueId);

        if (ballotId == null)
        {
            Console.WriteLine($"No ballotId found for user {userId} in league {leagueId}");
            return NotFound();
        }

        Console.WriteLine($"Found ballotId: {ballotId} for user {userId} in league {leagueId}");
        return Ok(ballotId);
    }

    [HttpGet]
    [Route("populateBallotContent")]
    [Produces("application/json")]
    public async Task<ActionResult<string[]>> GetBallotContentByIdAsync([FromQuery] int ballotId)
    {
        var ballotContent = await _database.GetBallotContentById(ballotId);

        return Ok(ballotContent);
    }

    [HttpGet]
    [Route("nextRaceId")]
    [Produces("application/json")]
    public async Task<ActionResult<int>> GetNextRaceIdAsync()
    {
        try
        {
            var nextRaceId = await _database.GetNextRaceId();
            if (nextRaceId.HasValue)
            {
                return new OkObjectResult(nextRaceId.Value);
            }
            return NotFound("No upcoming race found.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching next race ID.");
            return StatusCode(500, "An error occurred while fetching the next race ID.");
        }
    }

    [HttpGet]
    [Route("race/{raceId?}")]
    [Produces("application/json")]
    public async Task<ActionResult<List<Ballot>>> GetBallotsForRaceAsync(int? raceId)
    {
        try
        {
            // If raceId is not provided, fetch the next race ID
            if (!raceId.HasValue)
            {
                _logger.LogInformation("No raceId provided. Fetching the next race ID.");
                var nextRaceId = await _database.GetNextRaceId();

                if (!nextRaceId.HasValue)
                {
                    return NotFound("No upcoming race found.");
                }

                raceId = nextRaceId.Value;
            }

            _logger.LogInformation("Fetching ballots for race_id: {RaceId}", raceId.Value);

            // Fetch ballots by raceId
            var ballots = await _database.GetBallotsByRaceIdAsync(raceId.Value);

            if (ballots == null || ballots.Count == 0)
            {
                return NotFound($"No ballots found for race_id: {raceId.Value}.");
            }

            return Ok(ballots);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ballots for the race.");
            return StatusCode(500, "An error occurred while fetching ballots.");
        }
    }


}
