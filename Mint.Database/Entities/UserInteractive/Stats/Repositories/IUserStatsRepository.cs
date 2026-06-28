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
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User stats DTO or null if not found</returns>
    Task<UserStatsDto?> GetStatsByUserIdAsync(long userId, CancellationToken cancellationToken);

    /// <summary>
    /// Update user stats
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="dto">DTO with update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if stats were updated</returns>
    Task<bool> UpdateStatsAsync(long userId, UserStatsUpdateDto dto, CancellationToken cancellationToken);
}
