using System.Collections.ObjectModel;

namespace Mint.App.Services.System.News.Tavily;

/// <summary>
/// Settings for Tavily API
/// </summary>
public record TavilySettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "TavilySettings";

    /// <summary>
    /// Tavily API key (used as Bearer token)
    /// </summary>
    public required string ApiKey { get; init; } = string.Empty;

    /// <summary>
    /// Tavily API base URL
    /// </summary>
    public Uri ApiUrl { get; init; } = new("https://api.tavily.com/search");

    /// <summary>
    /// Number of search results to return
    /// </summary>
    public int MaxResults { get; init; } = 10;

    /// <summary>
    /// Search depth (basic, advanced)
    /// </summary>
    public string SearchDepth { get; init; } = "advanced";

    /// <summary>
    /// Search topic (news, general, etc.)
    /// </summary>
    public string? Topic { get; init; }

    /// <summary>
    /// Number of days back to search for news
    /// </summary>
    public int DaysBack { get; init; } = 30;

    /// <summary>
    /// Search time range (day, week, month, year)
    /// </summary>
    public string TimeRange { get; init; } = "month";

    /// <summary>
    /// Whether to filter by published date
    /// </summary>
    public bool? FilterByPublishedDate { get; init; }

    /// <summary>
    /// Domains to exclude from search results
    /// </summary>
    public Collection<string> ExcludeDomains { get; init; } = new();
}
