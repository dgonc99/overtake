using Overtake.Entities;

namespace Overtake.Interfaces;

public interface IDatabase
{
    Task<int> InsertAccountAsync(string username, string firstName, string lastName, string email, byte[] passwordHash);

    Task<Account> GetAccountByIdAsync(int accountId);

    Task<Account> GetAccountByUsernameAsync(string username);

    Task<int> InsertLeagueAsync(int ownerId, string name, bool isPublic);

    Task<RaceLeague> GetLeagueByIdAsync(int leagueId);

    Task<RaceLeague> GetLeagueByNameAsync(string name);

    // Not implemented, commenting out to fix github CI
    //Task<RaceLeague[]> PopulateLeaguesAsync(int accountId);

    Task<int> InsertLeagueMembershipAsync(int leagueId, int accountId);

    Task<RaceLeagueMembership> GetMembershipByIdAsync(int leagueId);

    Task<int> InsertLeagueInviteAsync(int leagueId, int inviteeId, DateTime requestTime, int status);

    Task<LeagueInvite> GetLeagueInviteByUserIdAsync(int inviteId);
}