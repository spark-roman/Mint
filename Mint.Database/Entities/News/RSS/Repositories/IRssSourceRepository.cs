using Mint.Database.Entities.News.RSS.Dto;

namespace Mint.Database.Entities.News.RSS.Repositories;

/// <summary>
/// Repository interface for working with RSS sources.
/// </summary>
public interface IRssSourceRepository
{
    /// <summary>
    /// Get all active RSS sources.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active RSS source DTOs</returns>
    Task<List<RssSourceDto>> GetActiveSourcesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get RSS source by ID.
    /// </summary>
    /// <param name="id">Source ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>RSS source DTO or null if not found</returns>
    Task<RssSourceDto?> GetByIdAsync(long id, CancellationToken cancellationToken);
}
