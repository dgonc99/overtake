using Overtake.Entities;
using Overtake.Models;
using Overtake.Models.Requests;

namespace Overtake.Interfaces;

public interface IDatabase
{
    Task<int> InsertAccountAsync(string username, string firstName, string lastName, string email, byte[] passwordHash);

    Task<Account> GetAccountByIdAsync(int accountId);

    Task<Account> GetAccountByUsernameAsync(string username);

    Task<int> InsertLeagueAsync(int ownerId, string name, bool isPublic);

    Task<RaceLeague> GetLeagueByIdAsync(int leagueId);

    Task<RaceLeague> GetLeagueByNameAsync(string name);

    Task<RaceLeagueInfo[]> PopulateLeaguesAsync(int accountId);

    Task<int> InsertLeagueMembershipAsync(int leagueId, int accountId);

    Task<RaceLeagueMembership> GetMembershipByIdAsync(int leagueId);

    Task<int> InsertLeagueInviteAsync(int leagueId, int inviteeId, DateTime requestTime, int status);

    Task<LeagueInvite> GetLeagueInviteByUserIdAsync(int inviteId);

    Task<int?> GetBallotByUserIdAsync(int accountId);

    Task<int?> GetBallotByUserIdAndLeagueIdAsync(int accountId, int leagueId);

    Task<BallotContent[]> GetBallotContentAsync(int ballotId);

    Task<int> InsertBallotAsync(int userId, int leagueId, int raceId, List<DriverPrediction> driverPredictions, int? totalScore);

    Task<bool> UpdateBallotAsync(int userId, int leagueId, int raceId, List<DriverPrediction> driverPredictions);

    Task<bool> UpdateBallotScoreAsync(int userId, int leagueId, int raceId, int score);

    Task<RaceLeagueInfo[]> GetPublicLeagues();

    Task<Driver[]> PopulateDriversAsync();

    Task<Driver> GetDriverMetadataByNumberAsync(int driverNumber);

    Task<Track> GetTrackDataByRoundAsync(int roundNumber);

    Task<Member[]> GetLeagueDetailsAsync(int leagueId);

    Task<int> InsertFriendRequest(int initiatorId, FriendRequest request);

    Task<FriendInfo[]> GetFriends(int userId);

    Task<FriendRequestInfo[]> GetFriendRequests(int userId);

    Task<UserInfo[]> PopulateUsers(int userId);

    Task UpdateFriendInviteStatus(int inviteId, int status);

    Task<RaceLeagueInfo> JoinLeagueAsyncByInvite(string invite, int user_id);

    Task<UserPoints> GetUserPoints(int userId);

    Task<int> CreateLeagueInvite(LeagueInviteRequest request);

    Task<LeagueInviteInfo[]> GetLeagueInvites(int userId);

    Task UpdateLeagueInviteStatus(int inviteId, int status);

    Task<LeagueRoundDetails[]> GetLeagueRoundDetails(int leagueId, int raceId);

    Task<string[]> GetBallotContentById(int ballotId);

    Task<bool> IsUserLeagueOwner(int userId, int leagueId);

    Task<string> getLeagueJoinCode(int leagueId);

    Task<bool> UpdateLeagueDetailsAsync(UpdateLeagueRequest request);

    Task<int?> GetNextRaceId();

    Task<List<Ballot>> GetBallotsByRaceIdAsync(int raceId);

    Task<RaceLeagueInfo[]> PopulateInvitableRaceLeagues(int userId, int inviteeId);

    Task<Team> GetTeamMetadataByIdAsync(string constructorId);

    Task<bool> UsernameExistsAsync(string username);

    Task<string> GetUserEmail(int userId);

    Task<bool> UpdateField(int userId, string fieldName, object fieldValue);
}