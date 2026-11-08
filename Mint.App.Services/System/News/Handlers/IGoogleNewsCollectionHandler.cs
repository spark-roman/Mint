using Mint.App.Services.System.News.Dto;

namespace Mint.App.Services.System.News.Handlers;

/// <summary>
/// Handler for Google news collection
/// </summary>
public interface IGoogleNewsCollectionHandler
{
    /// <summary>
    /// Collect news from GNews API
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection result</returns>
    Task<NewsCollectionResult> CollectAsync(CancellationToken cancellationToken);
}
