using Mint.Database.Entities.News.Dto;

namespace Mint.App.Services.System.News.Tavily;

/// <summary>
/// Interface for Tavily news search
/// </summary>
public interface ITavilyClient
{
    /// <summary>
    /// Search for news using Tavily API
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="categoryCode">Optional category code to tag results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of news items</returns>
    Task<List<NewsCreateDto>> SearchAsync(string query, string? categoryCode, CancellationToken cancellationToken);
}
