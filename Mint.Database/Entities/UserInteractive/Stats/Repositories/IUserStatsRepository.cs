using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Repositories;

/// <summary>
/// Repository interface for working with user stats
/// </summary>
public interface IUserStatsRepository
{
    /// <summary>
    /// Create user stats
    /// </summary>
    /// <param name="dto">DTO for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created stats</returns>
    Task<long> CreateStatsAsync(UserStatsCreateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Get user stats by user ID
    /// </summary>
    /// <param name="externaUserId">External user id</param>
    /// <param name="systemType">System type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User stats DTO or null if not found</returns>
    Task<UserStatsDto?> GetStatsByUserIdAsync(long externaUserId, byte systemType, CancellationToken cancellationToken);

    /// <summary>
    /// Update user stats
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dto">DTO with update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if stats were updated</returns>
    Task<bool> UpdateStatsAsync(long userId, UserStatsUpdateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Get top stats
    /// </summary>
    /// <param name="top">Top number of stats to return</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of top stats</returns>
    Task<List<UserStatsDto>> GetTopStatsByUserIdAsync(int top, CancellationToken cancellationToken);

    /// <summary>
    /// Gets user's rank position (1-based) by rank points.
    /// </summary>
    /// <param name="rankPoints">Rank points</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User rank</returns>
    Task<int> GetUserRankByPointsAsync(decimal rankPoints, CancellationToken cancellationToken);
}
