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
    ILogger<TavilyClient> logger) : ITavilyClient
{
    private readonly TavilySettings _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly ILogger<TavilyClient> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<List<NewsCreateDto>> SearchAsync(string query, string? categoryCode, CancellationToken cancellationToken)
    {
        var request = new
        {
            api_key = _settings.ApiKey,
            query,
            max_results = _settings.MaxResults,
            search_depth = _settings.SearchDepth,
            include_answer = _settings.IncludeAnswer,
            include_images = _settings.IncludeImages
        };

        var requestJson = JsonSerializer.Serialize(request);
        _logger.LogDebug("Tavily request: {RequestJson}", requestJson);

        using var content = new StringContent(requestJson, Encoding.UTF8, MediaTypeNames.Application.Json);

        using var response = await _httpClient.PostAsync(_settings.ApiUrl, content, cancellationToken);

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogDebug("Tavily response: {ResponseJson}", responseJson);

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

        var newsItems = apiResponse.Results
            .Select(r => new NewsCreateDto
            {
                Title = r.Title,
                Link = r.Url.ToString(),
                Description = r.Content,
                Content = r.Content,
                PublishedAt = r.PublicDate ?? DateTimeOffset.UtcNow,
                CategoryCode = categoryCode
            })
            .ToList();

        _logger.LogInformation(
            "Tavily search completed for query '{Query}': {Count} results",
            query,
            newsItems.Count);

        return newsItems;
    }
}
