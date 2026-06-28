using Mint.Database.Entities.UserInteractive.Bonuses.Dto;

namespace Mint.Database.Entities.UserInteractive.Bonuses.Repositories;

/// <summary>
/// Repository interface for working with user bonus stats
/// </summary>
public interface IUserBonusStatsRepository
{
    /// <summary>
    /// Create user bonus stats
    /// </summary>
    /// <param name="dto">DTO for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created stats</returns>
    Task<long> CreateStatsAsync(UserBonusStatsCreateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Get user bonus stats by user id
    /// </summary>
    /// <param name="externalUserId">External user id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User bonus stats DTO or null if not found</returns>
    Task<UserBonusStatsDto?> GetStatsByUserIdAsync(long externalUserId, CancellationToken cancellationToken);

    /// <summary>
    /// Update user bonus stats
    /// </summary>
    /// <param name="dto">DTO with update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if stats were updated</returns>
    Task<bool> UpdateStatsAsync(UserBonusStatsUpdateDto dto, CancellationToken cancellationToken);
}
