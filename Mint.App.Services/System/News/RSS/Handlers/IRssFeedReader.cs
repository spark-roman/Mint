using Mint.Database.Entities.News.Dto;

namespace Mint.App.Services.System.News.RSS.Handlers;

/// <summary>
/// Service for reading and parsing RSS feeds.
/// </summary>
public interface IRssFeedReader
{
    /// <summary>
    /// Reads and parses an RSS feed from the given URL.
    /// </summary>
    Task<List<NewsCreateDto>> ReadFeedAsync(Uri url, long sourceId, string? categoryCode, CancellationToken cancellationToken);
}
