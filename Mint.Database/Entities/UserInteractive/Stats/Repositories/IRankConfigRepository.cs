using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Repositories;

/// <summary>
/// Repository interface for working with rank configs
/// </summary>
public interface IRankConfigRepository
{
    /// <summary>
    /// Get rank config by ID
    /// </summary>
    /// <param name="rankId">Rank config ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rank config DTO or null if not found</returns>
    Task<RankConfigDto?> GetRankConfigByIdAsync(int rankId, CancellationToken cancellationToken);

    /// <summary>
    /// Get rank config by code
    /// </summary>
    /// <param name="code">Rank code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Rank config DTO or null if not found</returns>
    Task<RankConfigDto?> GetRankConfigByCodeAsync(string code, CancellationToken cancellationToken);

    /// <summary>
    /// Get all rank configs
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all rank configs</returns>
    Task<List<RankConfigDto>> GetRankConfigsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get the highest rank config
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Highest rank config DTO or null if not found</returns>
    Task<RankConfigDto?> GetHighestRankAsync(CancellationToken cancellationToken);
}
