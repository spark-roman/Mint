using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.Database.Entities.News.Dto;

namespace Mint.App.Services.System.News.Google;

/// <summary>
/// Custom HTTP client for GNews API (https://gnews.io/)
/// </summary>
public sealed class GoogleNewsClient(
    IOptions<GNewsSettings> settings,
    HttpClient httpClient,
    //TimeProvider timeProvider,
    ILogger<GoogleNewsClient> logger) : IGoogleNewsClient
{
    private readonly GNewsSettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    //private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    private readonly ILogger<GoogleNewsClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<List<NewsCreateDto>> SearchAsync(string query, string? categoryCode, CancellationToken cancellationToken)
    {
        var uri = new UriBuilder(_httpClient.BaseAddress!)
        {
            Path = "api/v4/search",
            Query = BuildQuery(
                query,
                _settings.ApiKey,
                _settings.MaxResults,
                _settings.Language,
                _settings.Country)

        }.Uri;

        _logger.LogDebug("GNews request: {Uri}", uri);

        try
        {
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogDebug("GNews response ({StatusCode}): {Content}", response.StatusCode, content);

            response.EnsureSuccessStatusCode();

            var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Check for information messages (free plan limitations)
            if (root.TryGetProperty("information", out var info) && info.ToString() is { Length: > 0 })
            {
                _logger.LogWarning("GNews information: {Info}", info);
            }

            var articles = root.TryGetProperty("articlesRemovedFromResponse", out var removed)
                && removed.TryGetProperty("historicalArticles", out var hist)
                ? $"Excluded: {hist}"
                : "";

            if (!string.IsNullOrWhiteSpace(articles))
                _logger.LogWarning("GNews articles excluded: {Articles}", articles);

            var articleList = root.TryGetProperty("articles", out var articlesProp)
                ? articlesProp.EnumerateArray().Select(a => new GNewsArticle
                {
                    Title = a.GetProperty("title").GetString() ?? string.Empty,
                    Description = a.TryGetProperty("description", out var desc) ? desc.GetString() : null,
                    Url = a.GetProperty("url").GetString() ?? string.Empty,
                    PublishedAt = DateTime.Parse(a.GetProperty("publishedAt").GetString() ?? string.Empty, null, global::System.Globalization.DateTimeStyles.RoundtripKind)
                })
                .ToList()
                : [];

            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken);

            if (articleList.Count == 0)
            {
                _logger.LogWarning("No results returned from GNews for query: {Query}", query);
                return [];
            }

            var newsItems = articleList
                .Where(a => !string.IsNullOrWhiteSpace(a.Url) && !string.IsNullOrWhiteSpace(a.Title))
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

            _logger.LogInformation("GNews search completed for query '{Query}': {Count} results", query, newsItems.Count);

            return newsItems;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GNews API error for query '{Query}'", query);
            throw new InvalidOperationException($"GNews API error: {ex.Message}");
        }
    }

    private static string BuildQuery(string query, string apiKey, int maxResults, string? language, string? country)
    {
        var parts = new List<string>();
        parts.Add($"q={Uri.EscapeDataString(query)}");
        parts.Add($"apikey={Uri.EscapeDataString(apiKey)}");
        parts.Add($"max={Uri.EscapeDataString(maxResults.ToString(CultureInfo.InvariantCulture))}");
        parts.Add("sortby=publishedAt");

        if (!string.IsNullOrWhiteSpace(language))
            parts.Add($"lang={Uri.EscapeDataString(language)}");

        if (!string.IsNullOrWhiteSpace(country))
            parts.Add($"country={Uri.EscapeDataString(country)}");

        return string.Join("&", parts);
    }
}

/// <summary>
/// GNews API article model
/// </summary>
internal sealed class GNewsArticle
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string Url { get; init; }
    public DateTime PublishedAt { get; init; }
}
