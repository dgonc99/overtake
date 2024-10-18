using System.Text;
using Microsoft.AspNetCore.Mvc;
using Overtake.Interfaces;
using Overtake.Models;
using Overtake.Models.Requests;
using Overtake.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks.Dataflow;


/// <summary>
/// API related to league operations.
/// </summary>
[Authorize]
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

        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var account = await _database.GetAccountByIdAsync(userId);

        int newLeagueId = await _database.InsertLeagueAsync(account.AccountId, request.Name, request.IsPublic);

        int membership = await _database.InsertLeagueMembershipAsync(newLeagueId, account.AccountId);

        return new OkObjectResult(new RaceLeagueInfo {LeagueId = newLeagueId, OwnerId = account.AccountId, Name = request.Name, IsPublic = request.IsPublic });

    }

    /// <summary>
    ///  Retrieves all race leagues for the current user.
    /// </summary>
    [HttpGet]
    [Route("populate")]
    [Produces("application/json")]
    public async Task<ActionResult<RaceLeagueInfo[]>> PopulateAsync()
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        RaceLeagueInfo[] leagues = await _database.PopulateLeaguesAsync(userId);

        return new OkObjectResult(leagues);
    }

    [HttpGet]
    [Route("populatepublic")]
    [Produces("application/json")]
    public async Task<ActionResult<RaceLeagueInfo[]>> PopulatePublicAsync()
    {
        RaceLeagueInfo[] leagues = await _database.GetPublicLeagues();

        return new OkObjectResult(leagues);
    }

    [HttpPost]
    [Route("join")]
    [Produces("application/json")]
    public async Task<ActionResult<int>> JoinLeagueAsync([FromBody] JoinLeagueRequest request)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var account = await _database.GetAccountByIdAsync(userId);

        int membership = await _database.InsertLeagueMembershipAsync(request.leagueId, account.AccountId);

        return new OkObjectResult(membership);
    }
    
}
