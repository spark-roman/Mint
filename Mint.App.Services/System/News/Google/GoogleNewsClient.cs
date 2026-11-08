using System.Globalization;
using Gnews;
using Gnews.Constants;
using Gnews.Requests;
using Gnews.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.Database.Entities.News.Dto;

namespace Mint.App.Services.System.News.Google;

/// <summary>
/// Client for GNews API
/// </summary>
public sealed class GoogleNewsClient(
    IOptions<GNewsSettings> settings,
    GnewsClient gnewsClient,
    TimeProvider timeProvider,
    ILogger<GoogleNewsClient> logger) : IGoogleNewsClient
{
    private readonly GNewsSettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly GnewsClient _gnewsClient = gnewsClient ?? throw new ArgumentNullException(nameof(gnewsClient));
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    private readonly ILogger<GoogleNewsClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<List<NewsCreateDto>> SearchAsync(string query, string? categoryCode, CancellationToken cancellationToken)
    {
        var request = new SearchRequest
        {
            Q = query,
            Max = _settings.MaxResults,
            Sortby = Sortby.Relevance,
            From = _timeProvider.GetUtcNow().AddDays(-_settings.DaysBack).DateTime,
            To = _timeProvider.GetUtcNow().DateTime,
            Lang = ParseLang(_settings.Language),
            Country = ParseCountry(_settings.Country)
        };

        try
        {
            _logger.LogInformation("Gnews request: {Requrst}", request);

            var response = await _gnewsClient.Search(request);
            var articles = response?.Articles ?? [];

            if (articles.Count == 0)
            {
                _logger.LogWarning("No results returned from GNews for query: {Query}", query);
                return [];
            }

            var newsItems = articles
                //.Where(a => !string.IsNullOrWhiteSpace(a.Url) && !string.IsNullOrWhiteSpace(a.Title))
                .Select(a => new NewsCreateDto
                {
                    Title = a.Title,
                    Link = a.Url,
                    Description = a.Description,
                    Content = a.Description,
                    PublishedAt = a.PublishedAt.ToUniversalTime(),
                    CategoryCode = categoryCode
                })
                .ToList();

            _logger.LogInformation(
                "GNews search completed for query '{Query}': {Count} results",
                query,
                newsItems.Count);

            return newsItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GNews API error for query '{Query}'", query);
            throw new InvalidOperationException($"GNews API error: {ex.Message}");
        }
    }

    private static Lang? ParseLang(string? lang)
    {
        if (string.IsNullOrWhiteSpace(lang)) return null;
        return Enum.TryParse<Lang>(lang, ignoreCase: true, out var result) ? result : null;
    }

    private static Country? ParseCountry(string? country)
    {
        if (string.IsNullOrWhiteSpace(country)) return null;
        return Enum.TryParse<Country>(country, ignoreCase: true, out var result) ? result : null;
    }
}
