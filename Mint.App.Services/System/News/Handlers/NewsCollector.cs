using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.News.Dto;
using Mint.App.Services.System.News.RSS.Handlers;
using Mint.Database.Entities.News.Dto;
using Mint.Database.Entities.News.Repositories;
using Mint.Database.Entities.News.RSS.Dto;
using Mint.Database.Entities.News.RSS.Repositories;
using Sagara.FeedReader.Extensions;

namespace Mint.App.Services.System.News.Handlers;

/// <inheritdoc cref="INewsCollector"/>
public sealed class NewsCollector(
    IRssSourceRepository rssSourceRepository,
    INewsRepository newsRepository,
    IRssFeedReader feedReader,
    ILogger<NewsCollector> logger) : INewsCollector
{
    private readonly IRssSourceRepository _rssSourceRepository = rssSourceRepository ?? throw new ArgumentNullException(nameof(rssSourceRepository));

    private readonly INewsRepository _newsRepository = newsRepository ?? throw new ArgumentNullException(nameof(newsRepository));

    private readonly IRssFeedReader _feedReader = feedReader ?? throw new ArgumentNullException(nameof(feedReader));

    private readonly ILogger<NewsCollector> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public async Task<NewsCollectionResult> CollectAllAsync(CancellationToken cancellationToken)
    {
        var sources = await _rssSourceRepository.GetActiveSourcesAsync(cancellationToken);

        _logger.LogInformation("Starting news collection from {Count} sources", sources.Count);

        var result = new NewsCollectionResult
        {
            TotalFetched = 0,
            NewSaved = 0,
            SkippedDuplicates = 0,
            FailedSources = 0,
            Errors = []
        };

        foreach (var source in sources)
        {
            var sourceResult = await CollectFromSourceInternalAsync(source, cancellationToken);
            result.TotalFetched += sourceResult.TotalFetched;
            result.NewSaved += sourceResult.NewSaved;
            result.SkippedDuplicates += sourceResult.SkippedDuplicates;
            result.Errors.AddRange(sourceResult.Errors);
        }

        _logger.LogInformation(
            "News collection completed. Total: {Total}, New: {New}, Duplicates: {Duplicates}, Failed: {Failed}",
            result.TotalFetched,
            result.NewSaved,
            result.SkippedDuplicates,
            result.FailedSources);

        return result;
    }

    /// <inheritdoc/>
    public async Task<NewsCollectionResult> CollectFromSourceAsync(long sourceId, CancellationToken cancellationToken)
    {
        var source = await _rssSourceRepository.GetByIdAsync(sourceId, cancellationToken);
        if (source == null)
            throw new InvalidOperationException($"RSS source with ID {sourceId} not found");

        return await CollectFromSourceInternalAsync(source, cancellationToken);
    }

    private async Task<NewsCollectionResult> CollectFromSourceInternalAsync(RssSourceDto source, CancellationToken cancellationToken)
    {
        var result = new NewsCollectionResult
        {
            TotalFetched = 0,
            NewSaved = 0,
            SkippedDuplicates = 0,
            FailedSources = 0,
            Errors = []
        };

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var items = await _feedReader.ReadFeedAsync(source.Url, source.Id, source.CategoryCode, cancellationToken);
            result.TotalFetched = items.Count;

            var newItems = new List<NewsCreateDto>();

            foreach (var item in items)
            {
                var exists = await _newsRepository.ExistsByLinkAsync(item.Link, cancellationToken);
                if (exists)
                {
                    result.SkippedDuplicates++;
                    continue;
                }

                newItems.Add(item);
            }

            if (newItems.Count > 0)
            {
                await _newsRepository.CreateManyAsync(new Collection<NewsCreateDto>(newItems), cancellationToken);
                result.NewSaved = newItems.Count;
            }

            _logger.LogDebug(
                "Source '{SourceName}': Fetched {Fetched}, New {New}, Duplicates {Duplicates}",
                source.Name,
                result.TotalFetched,
                result.NewSaved,
                result.SkippedDuplicates);
        }
        catch (Exception ex)
        {
            result.FailedSources++;
            result.Errors.Add(ex.Message);
        }
#pragma warning restore CA1031 // Do not catch general exception types

        return result;
    }
}
