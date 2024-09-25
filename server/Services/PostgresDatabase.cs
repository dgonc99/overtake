using Npgsql;
using Overtake.Entities;
using Overtake.Interfaces;

namespace Overtake.Services;

public class PostgresDatabase : IDatabase
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ILogger<PostgresDatabase> _logger;

    public PostgresDatabase(ILogger<PostgresDatabase> logger, IConfiguration configuration)
    {
        _logger = logger;

        var connectionString = configuration["Database"];
        if (connectionString is null)
        {
            throw new NullReferenceException("Postgres connection string not configured");
        }

        _dataSource = NpgsqlDataSource.Create(connectionString);
    }

    public async Task<int> InsertAccountAsync(string username, string firstName, string lastName, string email, byte[] passwordHash)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"INSERT INTO account (username, first_name, last_name, email, password_hash)
                VALUES (@username, @first_name, @last_name, @email, @password_hash)
                RETURNING account_id"
        );

        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("first_name", firstName);
        cmd.Parameters.AddWithValue("last_name", lastName);
        cmd.Parameters.AddWithValue("email", email);
        cmd.Parameters.AddWithValue("password_hash", passwordHash);

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();
        int newAccountId = reader.GetInt32(0);

        return newAccountId;
    }

    public async Task<Account> GetAccountByIdAsync(int accountId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT username, first_name, last_name, email, password_hash FROM account
                WHERE account_id=@account_id"
        );

        cmd.Parameters.AddWithValue("account_id", accountId);

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();

        byte[] passwordHash = new byte[32];
        reader.GetBytes(4, 0, passwordHash, 0, passwordHash.Length);

        return new Account
        {
            AccountId = accountId,
            Username = reader.GetString(0),
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3),
            PasswordHash = passwordHash,
        };
    }

    public async Task<Account> GetAccountByUsernameAsync(string username)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT account_id, first_name, last_name, email, password_hash FROM account
                WHERE username=@username"
        );

        cmd.Parameters.AddWithValue("username", username);

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();

        byte[] passwordHash = new byte[32];
        reader.GetBytes(4, 0, passwordHash, 0, passwordHash.Length);

        return new Account
        {
            AccountId = reader.GetInt32(0),
            Username = username,
            FirstName = reader.GetString(1),
            LastName = reader.GetString(2),
            Email = reader.GetString(3),
            PasswordHash = passwordHash,
        };
    }

    public async Task<int> InsertLeagueAsync(int ownerId, string name, bool isPublic)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"INSERT INTO raceLeague (owner_id, name, is_public, create_time)
                VALUES (@owner_id, @name, @is_public, @create_time)
                RETURNING league_id"
        );

        cmd.Parameters.AddWithValue("owner_id", ownerId);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("is_public", isPublic);
        cmd.Parameters.AddWithValue("create_time", new DateTime());

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();
        int newLeagueId = reader.GetInt32(0);

        return newLeagueId;
    }

    public async Task<RaceLeague> GetLeagueByIdAsync(int leagueId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT owner_id, name, is_public, create_time FROM raceLeague
                WHERE league_id=@league_id"
        );

        cmd.Parameters.AddWithValue("league_id", leagueId);

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();

        return new RaceLeague
        {
            LeagueId = leagueId,
            OwnerId = reader.GetInt32(0),
            Name = reader.GetString(1),
            IsPublic = reader.GetBoolean(2),
            CreateTime = reader.GetDateTime(3),
        };
    }

    public async Task<RaceLeague> GetLeagueByNameAsync(string name)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT league_id, owner_id, is_public, create_time FROM raceLeague
                WHERE name=@name"
        );

        cmd.Parameters.AddWithValue("name", name);

        await using var reader = await cmd.ExecuteReaderAsync();

        return new RaceLeague
        {
            LeagueId = reader.GetInt32(0),
            OwnerId = reader.GetInt32(1),
            Name = name,
            IsPublic = reader.GetBoolean(2),
            CreateTime = reader.GetDateTime(3),
        };
    }

    public async Task<int> InsertLeagueMembershipAsync(int leagueId, int accountId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"INSERT INTO raceLeagueMembership (league_id, user_id, join_time)
                VALUES (@league_id, @user_id, @join_time)
                RETURNING league_id"
        );

        cmd.Parameters.AddWithValue("league_id", leagueId);
        cmd.Parameters.AddWithValue("user_id", accountId);
        cmd.Parameters.AddWithValue("join_time", new DateTime());

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();
        int newLeagueId = reader.GetInt32(0);

        return newLeagueId;
    }

    public async Task<RaceLeagueMembership> GetMembershipByIdAsync(int leagueId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT league_id, user_id, join_time FROM raceLeagueMembership
                WHERE league_id=@league_id"
        );

        cmd.Parameters.AddWithValue("league_id", leagueId);

        await using var reader = await cmd.ExecuteReaderAsync();

        return new RaceLeagueMembership
        {
            LeagueId = leagueId,
            UserId = reader.GetInt32(1),
            JoinTime = reader.GetDateTime(2),
        };
    }

    public async Task<int> InsertLeagueInviteAsync(int leagueId, int inviteeId, DateTime requestTime, int status)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"INSERT INTO leagueInvite (league_id, invitee_id, request_time, status)
                VALUES (@league_id, @invitee_id, @request_time, @status)
                RETURNING league_id"
        );

        cmd.Parameters.AddWithValue("league_id", leagueId);
        cmd.Parameters.AddWithValue("invitee_id", inviteeId);
        cmd.Parameters.AddWithValue("request_time", requestTime);
        cmd.Parameters.AddWithValue("status", status);

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();
        int newInviteId = reader.GetInt32(0);

        return newInviteId;
    }

    public async Task<LeagueInvite> GetLeagueInviteByUserIdAsync(int inviteeId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT invite_id, league_id, invitee_id, request_time, status FROM leagueInvite
                WHERE invitee_id=@invitee_id"
        );

        cmd.Parameters.AddWithValue("invitee_id", inviteeId);

        await using var reader = await cmd.ExecuteReaderAsync();

        return new LeagueInvite
        {
            InviteId = reader.GetInt32(0),
            LeagueId = reader.GetInt32(1),
            InviteeId = inviteeId,
            RequestTime = reader.GetDateTime(3),
            Status = reader.GetInt32(4),
        };
    }
}
