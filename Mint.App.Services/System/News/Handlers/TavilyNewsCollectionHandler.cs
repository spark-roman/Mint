using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.App.Services.System.News.Dto;
using Mint.App.Services.System.News.Tavily;
using Mint.Database.Entities.News.Dto;
using Mint.Database.Entities.News.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;

namespace Mint.App.Services.System.News.Handlers;

/// <inheritdoc cref="ITavilyNewsCollectionHandler"/>
public sealed class TavilyNewsCollectionHandler(
    ITavilyClient tavilyClient,
    ICategoryRepository categoryRepository,
    INewsRepository newsRepository,
    IOptions<TavilySettings> settings,
    TimeProvider timeProvider,
    ILogger<TavilyNewsCollectionHandler> logger) : ITavilyNewsCollectionHandler
{
    private readonly ITavilyClient _tavilyClient = tavilyClient ?? throw new ArgumentNullException(nameof(tavilyClient));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly INewsRepository _newsRepository = newsRepository ?? throw new ArgumentNullException(nameof(newsRepository));
    private readonly TavilySettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    private readonly ILogger<TavilyNewsCollectionHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public async Task<NewsCollectionResult> CollectAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting news collection using Tavily (DaysBack: {DaysBack})", _settings.DaysBack);

        var result = new NewsCollectionResult
        {
            TotalFetched = 0,
            NewSaved = 0,
            SkippedDuplicates = 0,
            FailedSources = 0,
            Errors = []
        };

        // Calculate cutoff date based on DaysBack setting
        var cutoffDate = _timeProvider.GetUtcNow().AddDays(-_settings.DaysBack);
        _logger.LogDebug("Using cutoff date: {CutoffDate}", cutoffDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        // Get only recent links for deduplication (optimization for large datasets)
        var existingLinks = await _newsRepository.GetLinksSinceAsync(cutoffDate, cancellationToken);
        _logger.LogDebug("Loaded {LinkCount} existing links since {CutoffDate}", existingLinks.Count, cutoffDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        var categories = await _categoryRepository.GetAllActiveAsync(cancellationToken);
        _logger.LogInformation("Found {Count} active categories for Tavily collection", categories.Count);

        var allNewItems = new List<NewsCreateDto>();

        foreach (var category in categories)
        {
            try
            {
                var query = TavilyQueryGenerator.GenerateQuery(category, cutoffDate);
                _logger.LogDebug("Searching news for category '{CategoryCode}' with query: {Query}", category.Code, query);

                var newsItems = await _tavilyClient.SearchAsync(query, category.Code, cancellationToken);
                result.TotalFetched += newsItems.Count;

                var newItems = newsItems.Where(item => !existingLinks.Contains(item.Link)).ToList();
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
}
