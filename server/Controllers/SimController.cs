using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Overtake.Interfaces;
using Overtake.Models;
using Overtake.Models.Requests;
using Overtake.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[Route("api/sim")]
[ApiController]
public class SimController : ControllerBase
{
    private readonly IDatabase _database;
    private readonly ILogger<BallotController> _logger;

    public SimController(IDatabase database, ILogger<BallotController> logger)
    {
        _database = database;
        _logger = logger;
    }

    [HttpPost]
    [Route("create")]
    [Produces("application/json")]
    public async Task<ActionResult<int>> CreateAsync([FromBody] CreateSimBallotRequest request)
    {
        var driverPredictions = request.DriverPredictions.Select((name, index) => new DriverPrediction
        {
            DriverName = name,
            Position = index + 1
        }).ToList();

        string username = request.Username;

        int newSimBallotId = await _database.InsertSimBallotAsync(username, driverPredictions);

        return new OkObjectResult(newSimBallotId);
    }
}