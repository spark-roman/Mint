using Mint.App.Services.System.News.Dto;

namespace Mint.App.Services.System.News.Handlers;

/// <summary>
/// Service for collecting news from RSS sources.
/// </summary>
public interface INewsCollector
{
    /// <summary>
    /// Collects news from all active RSS sources.
    /// </summary>
    Task<NewsCollectionResult> CollectAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Collects news from a specific RSS source.
    /// </summary>
    Task<NewsCollectionResult> CollectFromSourceAsync(long sourceId, CancellationToken cancellationToken);
}
