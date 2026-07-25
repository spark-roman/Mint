using System.Globalization;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using Mint.Database.Entities.News.Dto;
using Sagara.FeedReader;

namespace Mint.App.Services.System.News.RSS.Handlers;

/// <inheritdoc cref="IRssFeedReader"/>
/// <summary>
/// 
/// </summary>
/// <param name="feedReader"></param>
/// <param name="logger"></param>
public sealed class RssFeedReader(FeedReader feedReader, ILogger<RssFeedReader> logger) : IRssFeedReader
{
    private readonly FeedReader _feedReader = feedReader;
    private readonly ILogger<RssFeedReader> _logger = logger;

    /// <inheritdoc/>
    public async Task<List<NewsCreateDto>> ReadFeedAsync(Uri url, long sourceId, string? categoryCode, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(url);

        _logger.LogDebug("Fetching RSS feed from {Url}", url);

        var feed = await _feedReader.ReadFromUrlAsync(url.ToString(), null, cancellationToken);

        if (feed == null || feed.Items == null || feed.Items.Count == 0)
        {
            _logger.LogWarning("No items found in feed from {Url}", url);
            return [];
        }

        var items = new List<NewsCreateDto>();

        foreach (var item in feed.Items)
        {
            var title = item.Title;
            var link = item.Link;

            if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(link))
                continue;

            var newsDto = new NewsCreateDto
            {
                RssSourceId = sourceId,
                Title = title,
                Link = link,
                Description = item.Description,
                Content = item.Content,
                Author = item.Author,
                CategoryCode = categoryCode,
                PublishedAt = item.PublishingDate ?? DateTimeOffset.UtcNow
            };

            items.Add(newsDto);
        }

        _logger.LogDebug("Parsed {Count} items from {Url}", items.Count, url);
        return items;
    }
}
