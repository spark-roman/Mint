using Mint.App.Services.System.News.Dto;

namespace Mint.App.Services.System.News.Handlers;

/// <summary>
/// Service for collecting news using Tavily API.
/// </summary>
public interface ITavilyNewsCollectionHandler
{
    /// <summary>
    /// Collects news from all active categories using Tavily.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection result</returns>
    Task<NewsCollectionResult> CollectAsync(CancellationToken cancellationToken);
}
