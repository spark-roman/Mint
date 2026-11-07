using Mint.App.Services.System.News.Dto;

namespace Mint.App.Services.System.News.Handlers;

/// <summary>
/// Service for collecting news from RSS sources.
/// </summary>
public interface IRssNewsCollectionHandler
{
    /// <summary>
    /// Collects news from all active RSS sources.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection result</returns>
    Task<NewsCollectionResult> CollectAsync(CancellationToken cancellationToken);
}
