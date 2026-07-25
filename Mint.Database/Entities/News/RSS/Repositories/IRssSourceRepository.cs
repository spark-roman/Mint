using Mint.Database.Entities.News.RSS.Dto;

namespace Mint.Database.Entities.News.RSS.Repositories;

/// <summary>
/// Repository for RSS source operations.
/// </summary>
public interface IRssSourceRepository
{
    /// <summary>Gets all active RSS sources.</summary>
    Task<List<RssSourceDto>> GetActiveSourcesAsync(CancellationToken cancellationToken);

    /// <summary>Gets an RSS source by its identifier.</summary>
    Task<RssSourceDto?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>Gets an RSS source by its URL.</summary>
    Task<RssSourceDto?> GetByUrlAsync(Uri url, CancellationToken cancellationToken);

    /// <summary>Creates a new RSS source.</summary>
    Task<RssSourceDto> CreateAsync(RssSourceCreateDto dto, CancellationToken cancellationToken);

    /// <summary>Updates an existing RSS source.</summary>
    Task<RssSourceDto> UpdateAsync(RssSourceDto dto, CancellationToken cancellationToken);

    /// <summary>Deletes an RSS source.</summary>
    Task DeleteAsync(long id, CancellationToken cancellationToken);
}
