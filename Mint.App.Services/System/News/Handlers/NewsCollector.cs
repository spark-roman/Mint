using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.News.Dto;
using Mint.App.Services.System.News.RSS.Handlers;
using Mint.App.Services.System.News.Tavily;
using Mint.Database.Entities.News.Dto;
using Mint.Database.Entities.News.Repositories;
using Mint.Database.Entities.News.RSS.Dto;
using Mint.Database.Entities.News.RSS.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;

namespace Mint.App.Services.System.News.Handlers;

/// <inheritdoc cref="INewsCollector"/>
public sealed class NewsCollector(
    IRssFeedReader rssFeedReader,
    IRssSourceRepository rssSourceRepository,
    ITavilyClient tavilyClient,
    ICategoryRepository categoryRepository,
    INewsRepository newsRepository,
    ILogger<NewsCollector> logger) : INewsCollector
{
    private readonly IRssFeedReader _rssFeedReader = rssFeedReader ?? throw new ArgumentNullException(nameof(rssFeedReader));
    private readonly IRssSourceRepository _rssSourceRepository = rssSourceRepository ?? throw new ArgumentNullException(nameof(rssSourceRepository));
    private readonly ITavilyClient _tavilyClient = tavilyClient ?? throw new ArgumentNullException(nameof(tavilyClient));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly INewsRepository _newsRepository = newsRepository ?? throw new ArgumentNullException(nameof(newsRepository));
    private readonly ILogger<NewsCollector> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public async Task<NewsCollectionResult> CollectFromRssAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting news collection from RSS sources");

        var result = new NewsCollectionResult
        {
            TotalFetched = 0,
            NewSaved = 0,
            SkippedDuplicates = 0,
            FailedSources = 0,
            Errors = []
        };

        var sources = await _rssSourceRepository.GetActiveSourcesAsync(cancellationToken);
        _logger.LogInformation("Found {Count} active RSS sources", sources.Count);

        foreach (var source in sources)
        {
            var sourceResult = await CollectFromSourceInternalAsync(source, cancellationToken);
            result.TotalFetched += sourceResult.TotalFetched;
            result.NewSaved += sourceResult.NewSaved;
            result.SkippedDuplicates += sourceResult.SkippedDuplicates;
            foreach (var error in sourceResult.Errors)
            {
                result.Errors.Add(error);
            }
        }

        _logger.LogInformation(
            "RSS collection completed. Total: {Total}, New: {New}, Duplicates: {Duplicates}, Failed: {Failed}",
            result.TotalFetched,
            result.NewSaved,
            result.SkippedDuplicates,
            result.FailedSources);

        return result;
    }

    /// <inheritdoc/>
    public async Task<NewsCollectionResult> CollectFromTavilyAsync(DateTimeOffset targetDate, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting news collection using Tavily");

        var result = new NewsCollectionResult
        {
            TotalFetched = 0,
            NewSaved = 0,
            SkippedDuplicates = 0,
            FailedSources = 0,
            Errors = []
        };

        // Load all existing links in one query
        var existingLinks = await _newsRepository.GetAllLinksAsync(cancellationToken);

        // Get all active categories
        var categories = await _categoryRepository.GetAllActiveAsync(cancellationToken);
        _logger.LogInformation("Found {Count} active categories for Tavily collection", categories.Count);

        var allNewItems = new List<NewsCreateDto>();

        foreach (var category in categories)
        {
            try
            {
                var query = TavilyQueryGenerator.GenerateQuery(category, targetDate);
                _logger.LogDebug("Searching news for category '{CategoryCode}' with query: {Query}", category.Code, query);

                var newsItems = await _tavilyClient.SearchAsync(query, category.Code, cancellationToken);
                result.TotalFetched += newsItems.Count;

                var newItems = FilterNewItems(newsItems, existingLinks);
                result.SkippedDuplicates += newsItems.Count - newItems.Count;

                allNewItems.AddRange(newItems);
            }
            catch (InvalidOperationException ex)
            {
                result.FailedSources++;
                result.Errors.Add($"Category '{category.Code}': {ex.Message}");
                _logger.LogError(ex, "Failed to collect news for category '{CategoryCode}'", category.Code);
            }
        }

        // Deduplicate across categories
        allNewItems = allNewItems.GroupBy(n => n.Link).Select(g => g.First()).ToList();

        if (allNewItems.Count > 0)
        {
            await _newsRepository.CreateManyAsync(new Collection<NewsCreateDto>(allNewItems), cancellationToken);
            result.NewSaved = allNewItems.Count;
        }

        _logger.LogInformation(
            "Tavily collection completed. Total: {Total}, New: {New}, Duplicates: {Duplicates}, Failed: {Failed}",
            result.TotalFetched,
            result.NewSaved,
            result.SkippedDuplicates,
            result.FailedSources);

        return result;
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

        try
        {
            var items = await _rssFeedReader.ReadFeedAsync(source.Url, source.Id, source.CategoryCode, cancellationToken);
            result.TotalFetched = items.Count;

            // Load all existing links in one query
            var existingLinks = await _newsRepository.GetAllLinksAsync(cancellationToken);

            var newItems = FilterNewItems(items, existingLinks);
            result.SkippedDuplicates = items.Count - newItems.Count;

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
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            result.FailedSources++;
            result.Errors.Add(ex.Message);
        }

        return result;
    }

    private static List<NewsCreateDto> FilterNewItems(IEnumerable<NewsCreateDto> items, HashSet<string> existingLinks)
    {
        return items.Where(item => !existingLinks.Contains(item.Link)).ToList();
    }
}
