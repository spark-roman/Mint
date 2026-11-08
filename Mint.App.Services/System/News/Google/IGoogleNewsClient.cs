using Mint.Database.Entities.News.Dto;

namespace Mint.App.Services.System.News.Google;

/// <summary>
/// Interface for Google news search
/// </summary>
public interface IGoogleNewsClient
{
    /// <summary>
    /// Search for news using GNews API
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="categoryCode">Optional category code to tag results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of news items</returns>
    Task<List<NewsCreateDto>> SearchAsync(string query, string? categoryCode, CancellationToken cancellationToken);
}
