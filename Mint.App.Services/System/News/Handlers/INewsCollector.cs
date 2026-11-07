using Mint.App.Services.System.News.Dto;

namespace Mint.App.Services.System.News.Handlers;

/// <summary>
/// Service for collecting news from RSS sources and Tavily API.
/// </summary>
public interface INewsCollector
{
    /// <summary>
    /// Collects news from all active RSS sources.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection result</returns>
    Task<NewsCollectionResult> CollectFromRssAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Collects news from all active categories using Tavily.
    /// </summary>
    /// <param name="targetDate">Date to use for news queries</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection result</returns>
    Task<NewsCollectionResult> CollectFromTavilyAsync(DateTimeOffset targetDate, CancellationToken cancellationToken);
}
