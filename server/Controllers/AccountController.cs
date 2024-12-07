using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Overtake.Interfaces;
using Overtake.Models;
using Overtake.Models.Requests;

namespace Overtake.Controllers;

/// <summary>
/// API related to account operations.
/// </summary>
[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IDatabase _database;
    private readonly ILogger<AccountController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AccountController(IDatabase database, ILogger<AccountController> logger, IConfiguration configuration, IEmailService emailService)
    {
        _database = database;
        _logger = logger;
        _configuration = configuration;
        _emailService = emailService;
    }

    /// <summary>
    /// Creates a new account.
    /// </summary>
    [HttpPost]
    [Route("register")]
    [Produces("application/json")]
    public async Task<ActionResult<Session>> RegisterAsync([FromBody] RegisterRequest request)
    {
        _logger.LogTrace(
            "Registration request: username={0}, firstname={1}, lastname={2}, email={3}",
            request.Username,
            request.FirstName,
            request.LastName,
            request.Email
        );

        if (string.IsNullOrWhiteSpace(request.Username)
            || string.IsNullOrWhiteSpace(request.FirstName)
            || string.IsNullOrWhiteSpace(request.LastName)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password))
        {
            return new BadRequestResult();
        }

        bool usernameExists = await _database.UsernameExistsAsync(request.Username);
        if (usernameExists)
        {
            return Conflict(new { message = $"The username {request.Username} is already taken." });
        }

        using var sha256 = SHA256.Create();
        byte[] passwordHash = sha256.ComputeHash(Encoding.ASCII.GetBytes("overtake|" + request.Password));

        int newUserId = await _database.InsertAccountAsync(request.Username, request.FirstName, request.LastName, request.Email, passwordHash);

        var token = GenerateJwtToken(newUserId.ToString());

        return new OkObjectResult(new Session
        {
            Token = token
        });
    }

    /// <summary>
    /// Creates a new session for an account by logging in.
    /// </summary>
    [HttpPost]
    [Route("login")]
    [Produces("application/json")]
    public async Task<ActionResult<Session>> LoginAsync([FromBody] LoginRequest request)
    {
        _logger.LogTrace(
            "Login request: username={0}",
            request.Username
        );

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return new BadRequestResult();
        }

        using var sha256 = SHA256.Create();
        byte[] requestPasswordHash = sha256.ComputeHash(Encoding.ASCII.GetBytes("overtake|" + request.Password));

        var account = await _database.GetAccountByUsernameAsync(request.Username);
        if (!requestPasswordHash.SequenceEqual(account.PasswordHash))
        {
            return new UnauthorizedResult();
        }

        var token = GenerateJwtToken(account.AccountId.ToString());

        return new OkObjectResult(new Session
        {
            Token = token
        });
    }

    /// <summary>
    /// Gets account information.
    /// </summary>
    [Authorize]
    [HttpGet]
    [Route("info")]
    [Produces("application/json")]
    public async Task<AccountInfo> InfoAsync()
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var account = await _database.GetAccountByIdAsync(userId);

        return new AccountInfo
        {
            Username = account.Username,
            UserId = account.AccountId,
        };
    }

    private string GenerateJwtToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    [Authorize]
    [HttpGet]
    [Route("pointsInfo")]
    [Produces("application/json")]
    public async Task<ActionResult<UserPoints>> GetUserPointsAsync([FromQuery] int userId)
    {
        var account = await _database.GetAccountByIdAsync(userId);

        var points = await _database.GetUserPoints(userId);

        return new OkObjectResult(new UserPoints
        {
            Username = account.Username,
            TotalPoints = points.TotalPoints,
            PointsThisSeason = points.PointsThisSeason,
            HighestLeaguePoints = points.HighestLeaguePoints,
            HighestLeagueName = points.HighestLeagueName
        });
    }

    [Authorize]
    [HttpPost]
    [Route("send-edit-email")]
    public async Task<IActionResult> SendEditEmail()
    {
        // Get the user ID from the authenticated user's claims
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // Fetch the user's email from the database
        var email = await _database.GetUserEmail(userId);
        if (string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Email not found for user ID: {UserId}", userId);
            return BadRequest("Email not found.");
        }

        // Define the link for the edit profile page
        string link = "http://localhost:3000/editprofile";

        try
        {
            // Log the email and link details
            _logger.LogInformation("Sending email to: {Email} with link: {Link}", email, link);

            // Send the email using the email service
            await _emailService.SendEmailAsync(email, "Edit Profile Link",
                $"Click the following link to edit your profile: {link}");

            // Return success response
            return Ok("Email sent successfully.");
        }
        catch (Exception ex)
        {
            // Log any errors encountered
            _logger.LogError(ex, "Error while sending email to {Email}", email);
            return StatusCode(500, $"Error sending email: {ex.Message}");
        }
    }

    [Authorize]
    [HttpPost]
    [Route("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        int userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        if (!string.IsNullOrEmpty(request.FirstName))
        {
            await _database.UpdateField(userId, "first_name", request.FirstName);
        }

        if (!string.IsNullOrEmpty(request.LastName))
        {
            await _database.UpdateField(userId, "last_name", request.LastName);
        }

        if (!string.IsNullOrEmpty(request.Username))
        {
            await _database.UpdateField(userId, "username", request.Username);
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            await _database.UpdateField(userId, "password", request.Password);
        }

        return Ok("Profile updated successfully.");
    }
}