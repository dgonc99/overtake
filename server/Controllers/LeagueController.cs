using System.Text;
using Microsoft.AspNetCore.Mvc;
using Overtake.Interfaces;
using Overtake.Models;
using Overtake.Models.Requests;
using Overtake.Entities;

/// <summary>
/// API related to league operations.
/// </summary>
[Route("api/league")]
[ApiController]
public class LeagueController : ControllerBase
{
    private readonly IDatabase _database;
    private readonly ILogger<LeagueController> _logger;

    public LeagueController(IDatabase database, ILogger<LeagueController> logger)
    {
        _database = database;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new race league
    /// </summary>
    [HttpPost]
    [Route("create")]
    [Produces("application/json")]
    public async Task<ActionResult<RaceLeagueInfo>> CreateAsync([FromBody] CreateLeagueRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return new BadRequestResult();
        }

        int userId = Convert.ToInt32(HttpContext.User.Claims.First(x => x.Type == "userId").Value);

        var account = await _database.GetAccountByIdAsync(userId);

        int newLeagueId = await _database.InsertLeagueAsync(account.AccountId, request.Name, request.IsPublic);

        int membership = await _database.InsertLeagueMembershipAsync(newLeagueId, account.AccountId);

        return new OkObjectResult(new RaceLeagueInfo { OwnerId = account.AccountId, Name = request.Name, IsPublic = request.IsPublic });

    }
}
