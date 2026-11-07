using System.Globalization;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.Database.Entities.News.Dto;

namespace Mint.App.Services.System.News.Tavily;

/// <summary>
/// Client for Tavily News Search API
/// </summary>
public sealed class TavilyClient(
    IOptions<TavilySettings> settings,
    HttpClient httpClient,
    TimeProvider timeProvider,
    ILogger<TavilyClient> logger) : ITavilyClient
{
    private readonly TavilySettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    private readonly ILogger<TavilyClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<List<NewsCreateDto>> SearchAsync(string query, string? categoryCode, CancellationToken cancellationToken)
    {
        var timeRange = CalculateTimeRange(_settings.DaysBack);

        var request = new
        {
            api_key = _settings.ApiKey,
            query,
            max_results = _settings.MaxResults,
            search_depth = _settings.SearchDepth,
            topic = _settings.Topic,
            time_range = timeRange,
            filter_by_published_date = _settings.FilterByPublishedDate
        };

        var requestJson = JsonSerializer.Serialize(request);
        _logger.LogInformation("Tavily request: {RequestJson}", requestJson);

        using var content = new StringContent(requestJson, Encoding.UTF8, MediaTypeNames.Application.Json);

        using var response = await _httpClient.PostAsync(_settings.ApiUrl, content, cancellationToken);

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("Tavily response: {ResponseJson}", responseJson);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Tavily API error: {StatusCode} - {Error}", response.StatusCode, responseJson);
            throw new InvalidOperationException($"Tavily API error: {response.StatusCode} - {responseJson}");
        }

        var apiResponse = JsonSerializer.Deserialize<TavilySearchResponse>(responseJson);

        if (apiResponse?.Results is null or { Count: 0 })
        {
            _logger.LogWarning("No results returned from Tavily for query: {Query}", query);
            return [];
        }

        var now = _timeProvider.GetUtcNow();
        var newsItems = apiResponse.Results
            .Select(r => new NewsCreateDto
            {
                Title = r.Title,
                Link = r.Url.ToString(),
                Description = r.Content,
                Content = r.Content,
                PublishedAt = ParseDate(r.PublicDateRaw, now),
                CategoryCode = categoryCode,
                RelevanceScore = r.Score
            })
            .ToList();

        _logger.LogInformation(
            "Tavily search completed for query '{Query}': {Count} results",
            query,
            newsItems.Count);

        return newsItems;
    }

    private static DateTimeOffset ParseDate(string? raw, DateTimeOffset fallback)
    {
        if (string.IsNullOrWhiteSpace(raw)) return fallback;

        if (DateTimeOffset.TryParse(raw, null, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var date))
            return date;

        return fallback;
    }

    private static string CalculateTimeRange(int daysBack)
    {
        if (daysBack <= 1) return "day";
        if (daysBack <= 7) return "week";
        if (daysBack <= 31) return "month";
        if (daysBack <= 365) return "year";
        return "year";
    }
}
