using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.Common.Contracts.Users;

namespace Mint.App.Services.UserInteractive.Leaderboards;

/// <summary>
/// Service for leaderboard operations.
/// </summary>
public interface ILeaderboardHandler
{
    /// <summary>
    /// Gets leaderboard data with pagination.
    /// </summary>
    /// <param name="top">Top amount of users to retrieve.</param>
    /// <param name="externalUserId">External user id.</param>
    /// <param name="authSystem">Auth system.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns></returns>
    Task<LeaderboardResultDto> GetLeaderboardAsync(int top, long externalUserId, AuthSystem authSystem, CancellationToken cancellationToken);
}
