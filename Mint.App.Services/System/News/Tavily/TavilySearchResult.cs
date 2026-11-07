using System.Text.Json.Serialization;

namespace Mint.App.Services.System.News.Tavily;

/// <summary>
/// Tavily API search result
/// </summary>
public sealed record TavilySearchResult
{
    /// <summary>
    /// Result title
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    /// <summary>
    /// Result URL
    /// </summary>
    [JsonPropertyName("url")]
    public required Uri Url { get; init; }

    /// <summary>
    /// Result content
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }

    /// <summary>
    /// Result relevance score
    /// </summary>
    [JsonPropertyName("score")]
    public double? Score { get; init; }

    /// <summary>
    /// Publication date (raw string — Tavily may return various ISO 8601 variants)
    /// </summary>
    [JsonPropertyName("published_date")]
    public string? PublicDateRaw { get; init; }
}
