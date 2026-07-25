using System.Collections.ObjectModel;
using Mint.Database.Entities.News.Dto;

namespace Mint.Database.Entities.News.Repositories;

/// <summary>
/// Repository for news operations.
/// </summary>
public interface INewsRepository
{
    /// <summary>Gets a news item by its identifier.</summary>
    Task<NewsDto?> GetByIdAsync(long id, CancellationToken cancellationToken);

    /// <summary>Gets a news item by its link.</summary>
    Task<NewsDto?> GetByLinkAsync(string link, CancellationToken cancellationToken);

    /// <summary>Creates a new news item.</summary>
    Task<NewsDto> CreateAsync(NewsCreateDto dto, CancellationToken cancellationToken);

    /// <summary>Creates multiple news items.</summary>
    Task<List<NewsDto>> CreateManyAsync(Collection<NewsCreateDto> dtos, CancellationToken cancellationToken);

    /// <summary>Gets unprocessed news items.</summary>
    Task<List<NewsDto>> GetUnprocessedAsync(int limit, string? categoryCode, CancellationToken cancellationToken);

    /// <summary>Marks news items as processed.</summary>
    Task MarkAsProcessedAsync(Collection<long> ids, CancellationToken cancellationToken);

    /// <summary>Gets news items published within the last N hours.</summary>
    Task<List<NewsDto>> GetRecentAsync(int hours, int limit, CancellationToken cancellationToken);

    /// <summary>Checks if a news item exists by link.</summary>
    Task<bool> ExistsByLinkAsync(string link, CancellationToken cancellationToken);
}
