using Npgsql;
using Overtake.Entities;
using Overtake.Interfaces;
using Overtake.Models;
using Overtake.Models.Requests;

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
            @"INSERT INTO raceLeague (owner_id, name, is_public, create_time, invite_code)
                VALUES (@owner_id, @name, @is_public, @create_time, @invite_code)
                RETURNING league_id"
        );

        cmd.Parameters.AddWithValue("owner_id", ownerId);
        cmd.Parameters.AddWithValue("name", name);
        cmd.Parameters.AddWithValue("is_public", isPublic);
        cmd.Parameters.AddWithValue("create_time", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("invite_code", GenerateInviteCode());

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();
        int newLeagueId = reader.GetInt32(0);

        return newLeagueId;
    }

    public async Task<RaceLeague> GetLeagueByIdAsync(int leagueId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT owner_id, name, is_public, create_time, invite_code FROM raceLeague
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
            InviteCode = reader.GetString(4),
        };
    }

    public async Task<RaceLeague> GetLeagueByNameAsync(string name)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT league_id, owner_id, is_public, create_time, invite_code FROM raceLeague
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
            InviteCode = reader.GetString(4),
        };
    }

    public async Task<RaceLeagueInfo[]> PopulateLeaguesAsync(int accountId)
    {
        var leagueIds = new List<int>();
        var userLeagues = new List<RaceLeagueInfo>();

        await using var cmd = _dataSource.CreateCommand(
            @"SELECT league_id FROM raceLeagueMembership
                where user_id=@accountId"
        );

        cmd.Parameters.AddWithValue("accountId", accountId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            leagueIds.Add(reader.GetInt32(0));
        }

        for (int i = 0; i < leagueIds.Count; i++)
        {
            await using var cmdLeague = _dataSource.CreateCommand(
                @"SELECT league_id, owner_id, name, is_public FROM raceLeague
                    where league_id=@league_id"
            );

            cmdLeague.Parameters.AddWithValue("league_id", leagueIds[i]);

            await using var leagueReader = await cmdLeague.ExecuteReaderAsync();

            if (await leagueReader.ReadAsync())
            {
                RaceLeagueInfo leagueInfo = new RaceLeagueInfo
                {
                    LeagueId = leagueReader.GetInt32(0),
                    OwnerId = leagueReader.GetInt32(1),
                    Name = leagueReader.GetString(2),
                    IsPublic = leagueReader.GetBoolean(3),
                };

                userLeagues.Add(leagueInfo);

            }
        }

        return userLeagues.ToArray();
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
        cmd.Parameters.AddWithValue("join_time", DateTime.UtcNow);

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

    public async Task<int> InsertBallotAsync(int userId, int leagueId, int raceId, List<DriverPrediction> driverPredictions, int? totalScore = null)
    {

        // References Dominic's methods above.

        // Step 1: Insert into ballot table
        using var cmd = _dataSource.CreateCommand(
            @"INSERT INTO ballot (league_id, race_id, user_id, create_time, score)
                VALUES (@league_id, @race_id, @user_id, NOW(), @score)
                RETURNING ballot_id"
        );

        cmd.Parameters.AddWithValue("league_id", leagueId);
        cmd.Parameters.AddWithValue("race_id", raceId);
        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("score", totalScore.HasValue ? (object)totalScore.Value : DBNull.Value);

        // Execute command and read the result
        int ballotId;
        await using (var reader = await cmd.ExecuteReaderAsync()) // Use ExecuteReaderAsync
        {
            await reader.ReadAsync(); // Read from the reader
            ballotId = reader.GetInt32(0); // Get the returned ballot ID
        }

        // Step 2: Insert into ballotContent table
        for (int i = 0; i < driverPredictions.Count; i++)
        {
            using var cmdContent = _dataSource.CreateCommand(
            @"INSERT INTO ballotContent (ballot_id, position, driver_name)
                VALUES (@ballot_id, @position, @driver_name)"

            );

            cmdContent.Parameters.AddWithValue("ballot_id", ballotId);
            cmdContent.Parameters.AddWithValue("position", driverPredictions[i].Position);
            cmdContent.Parameters.AddWithValue("driver_name", driverPredictions[i].DriverName);

            await using var contentReader = await cmdContent.ExecuteReaderAsync();

        }

        return ballotId; // Return the newly generated ballot ID
    }

    public async Task<bool> UpdateBallotAsync(int userId, int leagueId, int raceId, List<DriverPrediction> driverPredictions)
    {
        // Step 1: Retrieve the ballot ID based on user, league, and race
        int ballotId;
        using (var cmd = _dataSource.CreateCommand(
            @"SELECT ballot_id FROM ballot WHERE user_id = @user_id AND league_id = @league_id AND race_id = @race_id"
        ))
        {
            cmd.Parameters.AddWithValue("user_id", userId);
            cmd.Parameters.AddWithValue("league_id", leagueId);
            cmd.Parameters.AddWithValue("race_id", raceId);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                // Ballot does not exist; return false to indicate no update was performed
                return false;
            }
            ballotId = reader.GetInt32(0);
        }

        // Step 2: Delete existing entries in ballotContent for this ballot
        using (var cmdDeleteContent = _dataSource.CreateCommand(
            @"DELETE FROM ballotContent WHERE ballot_id = @ballot_id"
        ))
        {
            cmdDeleteContent.Parameters.AddWithValue("ballot_id", ballotId);
            await cmdDeleteContent.ExecuteNonQueryAsync();
        }

        // Step 3: Insert updated driver predictions into ballotContent
        foreach (var prediction in driverPredictions)
        {
            using var cmdContent = _dataSource.CreateCommand(
                @"INSERT INTO ballotContent (ballot_id, position, driver_name)
                VALUES (@ballot_id, @position, @driver_name)"
            );

            cmdContent.Parameters.AddWithValue("ballot_id", ballotId);
            cmdContent.Parameters.AddWithValue("position", prediction.Position);
            cmdContent.Parameters.AddWithValue("driver_name", prediction.DriverName);

            await cmdContent.ExecuteNonQueryAsync();
        }

        return true; // Return true to indicate the ballot was successfully updated
    }


    public async Task<int?> GetBallotByUserIdAsync(int accountId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT ballot_id FROM ballot
                WHERE user_id=@user_id 
                AND settle_time IS NULL
                ORDER BY create_time DESC
                LIMIT 1"
        );

        cmd.Parameters.AddWithValue("user_id", accountId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return reader.GetInt32(0);
        }

        return null;
    }

    public async Task<int?> GetBallotByUserIdAndLeagueIdAsync(int accountId, int leagueId)
    {

        await using var cmd = _dataSource.CreateCommand(
            @"SELECT ballot_id FROM ballot
                WHERE user_id=@user_id
                AND league_id = @league_id
                AND settle_time IS NULL
                ORDER BY create_time DESC
                LIMIT 1"
        );

        cmd.Parameters.AddWithValue("user_id", accountId);
        cmd.Parameters.AddWithValue("league_id", leagueId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return reader.GetInt32(0);
        }

        return null;

    }

    public async Task<BallotContent[]> GetBallotContentAsync(int ballotId)
    {
        var ballotContents = new List<BallotContent>();

        await using var cmd = _dataSource.CreateCommand(
            @"SELECT position, driver_name FROM ballotContent
                WHERE ballot_id=@ballot_id"
        );

        cmd.Parameters.AddWithValue("ballot_id", ballotId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var ballotContent = new BallotContent
            {
                Position = reader.GetInt32(0),
                DriverId = reader.GetString(1)
            };

            ballotContents.Add(ballotContent);
        }

        return ballotContents.ToArray();
    }

    public async Task<bool> UpdateBallotScoreAsync(int userId, int leagueId, int raceId, int score)
    {

        int ballotId;

        // Step 1: Retrieve the ballot ID based on user, league, and race
        using (var cmd = _dataSource.CreateCommand(
            @"SELECT ballot_id FROM ballot 
          WHERE user_id = @user_id AND league_id = @league_id AND race_id = @race_id"
        ))
        {
            cmd.Parameters.AddWithValue("user_id", userId);
            cmd.Parameters.AddWithValue("league_id", leagueId);
            cmd.Parameters.AddWithValue("race_id", raceId);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                // Ballot does not exist; return false to indicate no update was performed
                return false;
            }
            ballotId = reader.GetInt32(0);
        }

        // Step 2: Update the score in the ballot table for the retrieved ballotId
        using (var cmdUpdateScore = _dataSource.CreateCommand(
            @"UPDATE ballot SET score = @score WHERE ballot_id = @ballot_id"
        ))
        {
            cmdUpdateScore.Parameters.AddWithValue("score", score);
            cmdUpdateScore.Parameters.AddWithValue("ballot_id", ballotId);

            int rowsAffected = await cmdUpdateScore.ExecuteNonQueryAsync();
            return rowsAffected > 0; // Return true if the score was updated successfully
        }

    }

    public async Task<RaceLeagueInfo[]> GetPublicLeagues()
    {
        var publicLeagues = new List<RaceLeagueInfo>();
        bool isPublic = true;

        await using var cmdLeague = _dataSource.CreateCommand(
            @"SELECT league_id, owner_id, name, is_public FROM raceLeague
                where is_public=@isPublic"
        );

        cmdLeague.Parameters.AddWithValue("isPublic", isPublic);

        await using var leagueReader = await cmdLeague.ExecuteReaderAsync();

        while (await leagueReader.ReadAsync()) // Use while to iterate over all results
        {
            RaceLeagueInfo leagueInfo = new RaceLeagueInfo
            {
                LeagueId = leagueReader.GetInt32(0),
                OwnerId = leagueReader.GetInt32(1),
                Name = leagueReader.GetString(2),
                IsPublic = leagueReader.GetBoolean(3),
            };

            publicLeagues.Add(leagueInfo);
        }

        return publicLeagues.ToArray();

    }

    // Retrieve driver metadata by driver number
    public async Task<Driver> GetDriverMetadataByNumberAsync(int driverNumber)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT driver_id, driver_number, first_name, last_name, age, nationality, height, team_id, headshot_path, car_image_path, team_image_path, flag_image_path
          FROM driver
          WHERE driver_number=@driver_number OR @driver_number = (SELECT permanent_number FROM driver WHERE permanent_number IS NOT NULL AND permanent_number=@driver_number)"
        );

        cmd.Parameters.AddWithValue("driver_number", driverNumber);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Driver
            {
                DriverId = reader.GetInt32(0),         // Correct column for driver_id
                DriverNumber = reader.GetInt32(1),     // Correct column for driver_number
                FirstName = reader.GetString(2),
                LastName = reader.GetString(3),
                Age = reader.GetInt32(4),
                Nationality = reader.GetString(5),
                Height = reader.GetFloat(6),
                TeamId = reader.GetInt32(7),
                HeadshotPath = reader.GetString(8),
                CarImagePath = reader.GetString(9),
                TeamImagePath = reader.GetString(10),
                FlagImagePath = reader.GetString(11)
            };
        }

        return null; // Return null if no driver is found with the given driver number
    }


    public async Task<Driver[]> PopulateDriversAsync()
    {
        var drivers = new List<Driver>();

        await using var cmd = _dataSource.CreateCommand(
            @"SELECT driver_number, first_name, last_name, age, nationality, height, team_id, headshot_path, car_image_path, team_image_path, flag_image_path, permanent_number
              FROM driver"
        );

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var driver = new Driver
            {
                DriverId = reader.GetInt32(0),
                DriverNumber = reader.GetInt32(1),
                FirstName = reader.GetString(2),
                LastName = reader.GetString(3),
                Age = reader.GetInt32(4),
                Nationality = reader.GetString(5),
                Height = reader.GetFloat(6),
                TeamId = reader.GetInt32(7),
                HeadshotPath = reader.GetString(8),
                CarImagePath = reader.GetString(9),
                TeamImagePath = reader.GetString(10),
                FlagImagePath = reader.GetString(11),
                PermanentNumber = reader.IsDBNull(12) ? null : reader.GetInt32(12)
            };

            drivers.Add(driver);
        }

        return drivers.ToArray();
    }


    public async Task<Track> GetTrackDataByRoundAsync(int roundNumber)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT round_number, name, location, distance, turns, layout_image_path
                FROM track
                WHERE round_number=@round_number"
        );

        cmd.Parameters.AddWithValue("round_number", roundNumber);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Track
            {
                RoundNumber = reader.GetInt32(0),
                Name = reader.GetString(1),
                Location = reader.GetString(2),
                Distance = reader.GetDouble(3),
                Turns = reader.GetInt32(4),
                ImagePath = reader.GetString(5)
            };
        }

        return null;
    }

    public async Task<Member[]> GetLeagueDetailsAsync(int leagueId)
    {
        var members = new List<Member>();

        await using var cmd = _dataSource.CreateCommand(
            @"SELECT a.username, COALESCE(SUM(b.score), 0) AS total_score
              FROM account a
              JOIN raceLeagueMembership rlm
              ON a.account_id = rlm.user_id
              LEFT JOIN ballot b
              ON rlm.user_id = b.user_id AND rlm.league_id = b.league_id
              WHERE rlm.league_id = @league_id
              GROUP BY a.username"
        );

        cmd.Parameters.AddWithValue("league_id", leagueId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var member = new Member
            {
                Username = reader.GetString(0), // Get the username
                TotalScore = reader.GetInt32(1)  // Get the total score
            };

            members.Add(member); // Add each member object to the list
        }

        return members.ToArray(); // Return an array of Member objects
    }

    public async Task<RaceLeagueInfo> JoinLeagueAsyncByInvite(string invite, int user_id)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT league_id FROM raceleague
                WHERE invite_code=@invite"
        );

        cmd.Parameters.AddWithValue("invite", invite);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            int leagueId = reader.GetInt32(0);

            await using var cmdJoin = _dataSource.CreateCommand(
                @"INSERT INTO raceLeagueMembership (league_id, user_id, join_time)
                    VALUES (@league_id, @user_id, @join_time)"
            );

            cmdJoin.Parameters.AddWithValue("league_id", leagueId);
            cmdJoin.Parameters.AddWithValue("user_id", user_id);
            cmdJoin.Parameters.AddWithValue("join_time", DateTime.UtcNow);

            await cmdJoin.ExecuteNonQueryAsync();

            return new RaceLeagueInfo
            {
                LeagueId = leagueId,
                OwnerId = user_id,
                Name = "Test",
                IsPublic = true
            };
        }

        return null;
    }

    public static string GenerateInviteCode(int length = 10)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<int> InsertFriendRequest(int initiatorId, FriendRequest request)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"INSERT INTO friendInvite (initiator_id, invitee_id, message, request_time, status)
                VALUES (@initiator_id, @invitee_id, @message, @request_time, @status)
                RETURNING invite_id"
        );

        cmd.Parameters.AddWithValue("initiator_id", initiatorId);
        cmd.Parameters.AddWithValue("invitee_id", request.InviteeId);
        cmd.Parameters.AddWithValue("message", request.Message);
        cmd.Parameters.AddWithValue("request_time", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("status", 0);

        await using var reader = await cmd.ExecuteReaderAsync();

        await reader.ReadAsync();
        int newRequestId = reader.GetInt32(0);

        return newRequestId;
    }

    public async Task<FriendInfo[]> GetFriends(int userId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT 
                CASE 
                    WHEN initiator_id = @user_id THEN invitee_id
                    ELSE initiator_id
                END AS friend_id,
                a.username
            FROM friendInvite f
            JOIN account a ON a.account_id = 
                CASE 
                    WHEN f.initiator_id = @user_id THEN f.invitee_id
                    ELSE f.initiator_id
                END
            WHERE (f.initiator_id = @user_id OR f.invitee_id = @user_id) AND f.status = 1"
        );

        cmd.Parameters.AddWithValue("user_id", userId);

        var friends = new List<FriendInfo>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var friendInfo = new FriendInfo
            {
                FriendId = reader.GetInt32(0),
                FriendName = reader.GetString(1),
            };

            friends.Add(friendInfo);
        }

        return friends.ToArray();
    }

    public async Task<FriendRequestInfo[]> GetFriendRequests(int userId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT fi.invite_id, fi.initiator_id, a.username AS initiator_username
                FROM friendInvite fi
                JOIN account a ON fi.initiator_id = a.account_id
                WHERE fi.invitee_id = @invitee_id AND fi.status = 0"
        );

        cmd.Parameters.AddWithValue("invitee_id", userId);

        var friendRequests = new List<FriendRequestInfo>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var friendRequestInfo = new FriendRequestInfo
            {
                InviteId = reader.GetInt32(0),
                InitiatorId = reader.GetInt32(1),
                InitiatorUsername = reader.GetString(2),
            };

            friendRequests.Add(friendRequestInfo);
        }

        return friendRequests.ToArray();
    }

    public async Task<UserInfo[]> PopulateUsers(int userId)
    {
        await using var cmd = _dataSource.CreateCommand(
        @"SELECT account_id, username
          FROM account a
          WHERE NOT EXISTS (
              SELECT 1
              FROM friendInvite fi
              WHERE (fi.initiator_id = @user_id AND fi.invitee_id = a.account_id)
                 OR (fi.initiator_id = a.account_id AND fi.invitee_id = @user_id)
          )
          AND a.account_id != @user_id"
        );

        cmd.Parameters.AddWithValue("user_id", userId);

        var users = new List<UserInfo>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var userInfo = new UserInfo
            {
                UserId = reader.GetInt32(0),
                Username = reader.GetString(1),
            };

            users.Add(userInfo);
        }

        return users.ToArray();
    }

    public async Task UpdateFriendInviteStatus(int inviteId, int status)
    {
        if (status == 1)
        {
            await using var cmd = _dataSource.CreateCommand(
                @"UPDATE friendInvite
                    SET status = @status
                    WHERE invite_id = @invite_id"
            );

            cmd.Parameters.AddWithValue("invite_id", inviteId);
            cmd.Parameters.AddWithValue("status", status);

            await cmd.ExecuteNonQueryAsync();
        }
        else if (status == 2)
        {
            await using var cmd = _dataSource.CreateCommand(
                @"DELETE FROM friendInvite
                    WHERE invite_id = @invite_id"
            );

            cmd.Parameters.AddWithValue("invite_id", inviteId);

            await cmd.ExecuteNonQueryAsync();
        }
    }

    public async Task<UserPoints> GetUserPoints(int userId)
    {
        var userPoints = new UserPoints
        {
            Username = string.Empty,
            TotalPoints = 0,
            PointsThisSeason = 0,
            HighestLeaguePoints = 0,
            HighestLeagueName = string.Empty,
        };

        // Get the total score of all ballots created by the user
        await using var cmdTotalPoints = _dataSource.CreateCommand(
            @"SELECT COALESCE(SUM(score), 0) AS total_points
          FROM ballot
          WHERE user_id = @user_id"
        );
        cmdTotalPoints.Parameters.AddWithValue("@user_id", userId);
        userPoints.TotalPoints = Convert.ToInt32(await cmdTotalPoints.ExecuteScalarAsync() ?? 0);

        // Get the total score of all ballots created by the user during the current year
        await using var cmdPointsThisSeason = _dataSource.CreateCommand(
            @"SELECT COALESCE(SUM(score), 0) AS points_this_season
          FROM ballot
          WHERE user_id = @user_id
          AND EXTRACT(YEAR FROM create_time) = EXTRACT(YEAR FROM CURRENT_DATE)"
        );
        cmdPointsThisSeason.Parameters.AddWithValue("@user_id", userId);
        userPoints.PointsThisSeason = Convert.ToInt32(await cmdPointsThisSeason.ExecuteScalarAsync() ?? 0);

        // Get the highest scoring league and its points for the user
        await using var cmdHighestLeague = _dataSource.CreateCommand(
            @"SELECT rl.name AS league_name, COALESCE(SUM(b.score), 0) AS league_score
          FROM raceleague rl
          JOIN raceLeagueMembership rlm ON rl.league_id = rlm.league_id
          LEFT JOIN ballot b ON rlm.league_id = b.league_id AND b.user_id = rlm.user_id
          WHERE rlm.user_id = @user_id
          GROUP BY rl.name
          ORDER BY league_score DESC
          LIMIT 1"
        );
        cmdHighestLeague.Parameters.AddWithValue("@user_id", userId);

        await using var reader = await cmdHighestLeague.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            userPoints.HighestLeagueName = reader.GetString(0); // Get the league name
            userPoints.HighestLeaguePoints = reader.GetInt32(1); // Get the league score
        }
        else
        {
            userPoints.HighestLeagueName = "N/A";
            userPoints.HighestLeaguePoints = 0;
        }

        return userPoints;
    }

    public async Task<int> CreateLeagueInvite(LeagueInviteRequest request)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"INSERT INTO leagueInvite (league_id, invitee_id, request_time, status)
              VALUES (@league_id, @invitee_id, @request_time, @status)
              RETURNING invite_id"
        );

        cmd.Parameters.AddWithValue("league_id", request.LeagueId);
        cmd.Parameters.AddWithValue("invitee_id", request.InviteeId);
        cmd.Parameters.AddWithValue("request_time", DateTime.UtcNow);
        cmd.Parameters.AddWithValue("status", 0);

        await using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        int newInviteId = reader.GetInt32(0);
        return newInviteId;
    }

    public async Task<LeagueInviteInfo[]> GetLeagueInvites(int userId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT li.invite_id, li.league_id, rl.name AS league_name
              FROM leagueInvite li
              JOIN raceleague rl ON li.league_id = rl.league_id
              WHERE li.invitee_id = @invitee_id AND li.status = 0"
        );

        cmd.Parameters.AddWithValue("invitee_id", userId);

        var leagueInvites = new List<LeagueInviteInfo>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var leagueInviteInfo = new LeagueInviteInfo
            {
                InviteId = reader.GetInt32(0),
                LeagueId = reader.GetInt32(1),
                LeagueName = reader.GetString(2),
            };

            leagueInvites.Add(leagueInviteInfo);
        }

        return leagueInvites.ToArray();
    }

    public async Task UpdateLeagueInviteStatus(int inviteId, int status)
    {
        if (status == 1)
        {
            await using var cmd = _dataSource.CreateCommand(
                @"INSERT INTO raceLeagueMembership (league_id, user_id, join_time)
                  SELECT league_id, invitee_id, NOW()
                  FROM leagueInvite
                  WHERE invite_id = @inviteId;"
            );

            cmd.Parameters.AddWithValue("inviteId", inviteId);

            await cmd.ExecuteNonQueryAsync();
        }

        await using var deleteCmd = _dataSource.CreateCommand(
            @"DELETE FROM leagueInvite
                WHERE invite_id = @inviteId"
        );

        deleteCmd.Parameters.AddWithValue("inviteId", inviteId);

        await deleteCmd.ExecuteNonQueryAsync();
    }

    public async Task<LeagueRoundDetails[]> GetLeagueRoundDetails(int leagueId, int raceId)
    {
        // Create the command with the SQL query
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT b.ballot_id, b.user_id, a.username, b.score
              FROM ballot b
              JOIN account a ON b.user_id = a.account_id
              WHERE b.league_id = @league_id AND b.race_id = @race_id"
        );

        // Add parameters
        cmd.Parameters.AddWithValue("league_id", leagueId);
        cmd.Parameters.AddWithValue("race_id", raceId);

        // Initialize the list to store results
        var ballots = new List<LeagueRoundDetails>();

        // Execute the command and read results
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            // Create a LeagueRoundDetails object and populate its properties
            var leagueRoundDetails = new LeagueRoundDetails
            {
                BallotId = reader.GetInt32(0),
                UserId = reader.GetInt32(1),
                Username = reader.GetString(2),
                // Explicitly cast nullable int to int or handle null
                Score = reader.IsDBNull(3) ? null : reader.GetFieldValue<int?>(3) ?? default,
            };

            // Add the object to the list
            ballots.Add(leagueRoundDetails);
        }

        // Return the list as an array
        return ballots.ToArray();
    }

    public async Task<string[]> GetBallotContentById(int ballotId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT driver_name
              FROM ballotContent
              WHERE ballot_id = @ballot_id
              ORDER BY position ASC"
        );

        cmd.Parameters.AddWithValue("ballot_id", ballotId);

        var driverNames = new List<string>();

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            driverNames.Add(reader.GetString(0));
        }

        return driverNames.ToArray();
    }

    public async Task<bool> IsUserLeagueOwner(int userId, int leagueId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT CASE
                  WHEN owner_id = @user_id THEN true
                  ELSE false
              END
              FROM raceleague
              WHERE league_id = @league_id"
        );

        cmd.Parameters.AddWithValue("user_id", userId);
        cmd.Parameters.AddWithValue("league_id", leagueId);

        var isOwner = false;

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            isOwner = reader.GetBoolean(0);
        }

        return isOwner;
    }

    public async Task<string> getLeagueJoinCode(int leagueId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT invite_code FROM raceLeague
                WHERE league_id = @league_id"
        );

        cmd.Parameters.AddWithValue("league_id", leagueId);

        string inviteCode = null;

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            inviteCode = reader.GetString(0);
        }

        return inviteCode;
    }

    public async Task<bool> UpdateLeagueDetailsAsync(UpdateLeagueRequest request)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"UPDATE raceleague
              SET name = @name, is_public = @is_public
              WHERE league_id = @league_id"
        );

        cmd.Parameters.AddWithValue("league_id", request.LeagueId);
        cmd.Parameters.AddWithValue("name", request.Name);
        cmd.Parameters.AddWithValue("is_public", request.IsPublic);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<int?> GetNextRaceId()
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT race_id
                FROM race
                WHERE DATE(start_time AT TIME ZONE 'MST') = DATE(NOW() AT TIME ZONE 'MST')
                LIMIT 1"
        );

        await using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return reader.GetInt32(0); // Get the race_id from the first column
        }

        return null; // No upcoming race found

    }

    public async Task<List<Ballot>> GetBallotsByRaceIdAsync(int raceId)
    {

        _logger.LogInformation("Fetching ballots for race_id in GetBallotsByRaceIdAsync: {RaceId}", raceId);

        var ballots = new List<Ballot>();

        await using var cmd = _dataSource.CreateCommand(
            @"SELECT ballot_id, league_id, user_id, create_time, settle_time, score
          FROM ballot
          WHERE race_id = @race_id"
        );

        cmd.Parameters.AddWithValue("race_id", raceId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var ballotId = reader.GetInt32(0);

            // Fetch driver predictions for this ballot
            var driverPredictions = await GetBallotContentAsync(ballotId);

            var ballot = new Ballot
            {
                BallotId = ballotId,
                LeagueId = reader.GetInt32(1),
                RaceId = raceId,
                UserId = reader.GetInt32(2),
                CreateTime = reader.GetDateTime(3),
                SettleTime = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                Score = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                DriverPredictions = driverPredictions.ToList()
            };

            ballots.Add(ballot);
        }

        return ballots;
    }

    // Retrieve team metadata by constructor id (from ergast)
    public async Task<Team> GetTeamMetadataByIdAsync(string constructorId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT team_id, name, full_name, nationality, base, team_chief, technical_chief, chassis, power_unit, car_image_path, team_image_path, flag_image_path, first_year, constructor_id
          FROM team
          WHERE constructor_id = @constructor_id"
        );

        cmd.Parameters.AddWithValue("constructor_id", constructorId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Team
            {
                TeamId = reader.GetInt32(0),
                Name = reader.GetString(1),
                FullName = reader.GetString(2),
                Nationality = reader.GetString(3),
                Base = reader.GetString(4),
                TeamChief = reader.GetString(5),
                TechnicalChief = reader.GetString(6),
                Chassis = reader.GetString(7),
                PowerUnit = reader.GetString(8),
                CarImagePath = reader.GetString(9),
                TeamImagePath = reader.GetString(10),
                FlagImagePath = reader.GetString(11),
                FirstYear = reader.GetInt32(12),
                ConstructorId = reader.GetString(13),
            };
        }

        return null; // Return null if no team is found with the given constructor id
    }

    public async Task<RaceLeagueInfo[]> PopulateInvitableRaceLeagues(int userId, int inviteeId)
    {
        var leagueIds = new List<int>();
        var invitableLeagues = new List<RaceLeagueInfo>();

        // Query to get league IDs where userId is a member,
        // inviteeId is not a member, and no existing invite exists for inviteeId in leagueInvite
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT rl.league_id
              FROM raceLeagueMembership rl
              WHERE rl.user_id = @userId
              AND rl.league_id NOT IN (
                  SELECT league_id 
                  FROM raceLeagueMembership 
                  WHERE user_id = @inviteeId
              )
              AND rl.league_id NOT IN (
                  SELECT league_id 
                  FROM leagueInvite 
                  WHERE invitee_id = @inviteeId
              )"
            );

        cmd.Parameters.AddWithValue("userId", userId);
        cmd.Parameters.AddWithValue("inviteeId", inviteeId);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            leagueIds.Add(reader.GetInt32(0));
        }

        // Fetch details of the leagues where the conditions are met
        for (int i = 0; i < leagueIds.Count; i++)
        {
            await using var cmdLeague = _dataSource.CreateCommand(
                @"SELECT league_id, owner_id, name, is_public 
              FROM raceLeague 
              WHERE league_id = @league_id"
            );

            cmdLeague.Parameters.AddWithValue("league_id", leagueIds[i]);

            await using var leagueReader = await cmdLeague.ExecuteReaderAsync();

            if (await leagueReader.ReadAsync())
            {
                RaceLeagueInfo leagueInfo = new RaceLeagueInfo
                {
                    LeagueId = leagueReader.GetInt32(0),
                    OwnerId = leagueReader.GetInt32(1),
                    Name = leagueReader.GetString(2),
                    IsPublic = leagueReader.GetBoolean(3),
                };

                invitableLeagues.Add(leagueInfo);
            }
        }

        return invitableLeagues.ToArray();
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT EXISTS (SELECT 1 FROM account WHERE username = @username)"
        );

        cmd.Parameters.AddWithValue("username", username);

        return (bool)await cmd.ExecuteScalarAsync();
    }

    public async Task<string?> GetUserEmail(int userId)
    {
        await using var cmd = _dataSource.CreateCommand(
            @"SELECT email FROM account WHERE account_id = @userId"
        );

        cmd.Parameters.AddWithValue("userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            var email = reader.IsDBNull(0) ? null : reader.GetString(0);
            Console.WriteLine($"Retrieved email: {email}"); // Log for debugging
            return email;
        }

        Console.WriteLine($"No email found for userId: {userId}"); // Log for debugging
        return null;
    }

    public async Task<bool> UpdateField(int userId, string fieldName, object? fieldValue)
    {
        var allowedFields = new HashSet<string> { "first_name", "last_name", "username", "password" };
        if (!allowedFields.Contains(fieldName.ToLower()))
        {
            throw new ArgumentException($"Field '{fieldName}' is not allowed to be updated.", nameof(fieldName));
        }

        var query = $@"
        UPDATE account
        SET {fieldName} = @fieldValue
        WHERE account_id = @userId";

        await using var cmd = _dataSource.CreateCommand(query);

        cmd.Parameters.AddWithValue("fieldValue", fieldValue ?? DBNull.Value);
        cmd.Parameters.AddWithValue("userId", userId);

        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<int> InsertSimBallotAsync(string username, List<DriverPrediction> driverPredictions)
    {
        using var cmd = _dataSource.CreateCommand(
            @"INSERT INTO simBallot (username, score)
                VALUES (@username, @score)
                RETURNING simballot_id"
        );

        cmd.Parameters.AddWithValue("username", username);
        cmd.Parameters.AddWithValue("score", DBNull.Value);

        int ballotId;
        await using (var reader = await cmd.ExecuteReaderAsync()) // Use ExecuteReaderAsync
        {
            await reader.ReadAsync(); // Read from the reader
            ballotId = reader.GetInt32(0); // Get the returned ballot ID
        }

        for (int i = 0; i < driverPredictions.Count; i++)
        {
            using var cmdContent = _dataSource.CreateCommand(
            @"INSERT INTO simballotContent (simballot_id, position, driver_name)
                VALUES (@ballot_id, @position, @driver_name)"
            );

            cmdContent.Parameters.AddWithValue("ballot_id", ballotId);
            cmdContent.Parameters.AddWithValue("position", driverPredictions[i].Position);
            cmdContent.Parameters.AddWithValue("driver_name", driverPredictions[i].DriverName);

            await using var contentReader = await cmdContent.ExecuteReaderAsync();

        }

        return ballotId;
    }
}