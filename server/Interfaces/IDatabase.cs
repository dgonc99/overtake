using Overtake.Entities;
using Overtake.Models;

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

    Task<Driver[]> PopulateDriversAsync();

    Task<Driver> GetDriverMetadataByNumberAsync(int driverNumber);
}