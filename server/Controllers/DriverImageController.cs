using System.Text;
using Microsoft.AspNetCore.Mvc;
using Overtake.Interfaces;
using Overtake.Models;
using Overtake.Models.Requests;
using Overtake.Entities;


[Route("api/driver")]
[ApiController]
public class DriverImageController : ControllerBase
{
    private readonly IDatabase _database;
    private readonly ILogger<DriverImageController> _logger;

    public DriverImageController(IDatabase database, ILogger<DriverImageController> logger)
    {
        _database = database;
        _logger = logger;
    }

    [HttpGet]
    [Route("populate")]
    [Produces("application/json")]
    public async Task<ActionResult<Driver[]>> PopulateAsync()
    {
        Driver[] drivers = await _database.PopulateDriversAsync();
        return new OkObjectResult(drivers);
    }

    [HttpGet]
    [Route("headshot/{driverNumber}")]
    [Produces("application/json")]
    public async Task<ActionResult<string>> GetDriverHeadshotAsync(int driverNumber)
    {
        var driverMetadata = await _database.GetDriverMetadataByNumberAsync(driverNumber);

        if (driverMetadata == null)
        {
            return NotFound();
        }

        // Return the relative path to the driver's headshot
        return new OkObjectResult(driverMetadata.HeadshotPath);
    }
}
