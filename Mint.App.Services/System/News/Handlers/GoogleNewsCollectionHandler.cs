using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.App.Services.System.News.Dto;
using Mint.App.Services.System.News.Google;
using Mint.App.Services.System.News.Tavily;
using Mint.Database.Entities.News.Dto;
using Mint.Database.Entities.News.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;

namespace Mint.App.Services.System.News.Handlers;

/// <summary>
/// Handler for collecting news from Google/GNews API
/// </summary>
public sealed class GoogleNewsCollectionHandler(
    IGoogleNewsClient googleNewsClient,
    ICategoryRepository categoryRepository,
    INewsRepository newsRepository,
    IOptions<GNewsSettings> settings,
    TimeProvider timeProvider,
    ILogger<GoogleNewsCollectionHandler> logger) : IGoogleNewsCollectionHandler
{
    private readonly IGoogleNewsClient _googleNewsClient = googleNewsClient ?? throw new ArgumentNullException(nameof(googleNewsClient));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly INewsRepository _newsRepository = newsRepository ?? throw new ArgumentNullException(nameof(newsRepository));
    private readonly GNewsSettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    private readonly ILogger<GoogleNewsCollectionHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public async Task<NewsCollectionResult> CollectAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting news collection using GNews (DaysBack: {DaysBack})", _settings.DaysBack);

        var result = new NewsCollectionResult
        {
            TotalFetched = 0,
            NewSaved = 0,
            SkippedDuplicates = 0,
            FailedSources = 0,
            Errors = []
        };

        var cutoffDate = _timeProvider.GetUtcNow().AddDays(-_settings.DaysBack);
        _logger.LogDebug("Using cutoff date: {CutoffDate}", cutoffDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        var existingLinks = await _newsRepository.GetLinksSinceAsync(cutoffDate, cancellationToken);
        _logger.LogDebug("Loaded {LinkCount} existing links since {CutoffDate}", existingLinks.Count, cutoffDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));

        var categories = await _categoryRepository.GetAllActiveForAIAsync(cancellationToken);
        _logger.LogInformation("Found {Count} active categories for GNews collection", categories.Count);

        var allNewItems = new List<NewsCreateDto>();

        foreach (var category in categories)
        {
            try
            {
                var query = TavilyQueryGenerator.GenerateQuery(category);
                _logger.LogDebug("Searching news for category '{CategoryCode}' with query: {Query}", category.Code, query);

                var newsItems = await _googleNewsClient.SearchAsync(query, category.Code, cancellationToken);
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
            "GNews collection completed. Total: {Total}, New: {New}, Duplicates: {Duplicates}, Failed: {Failed}",
            result.TotalFetched,
            result.NewSaved,
            result.SkippedDuplicates,
            result.FailedSources);

        return result;
    }
}
