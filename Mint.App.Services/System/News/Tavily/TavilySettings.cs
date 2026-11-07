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
    /// Include answer in search results
    /// </summary>
    public bool IncludeAnswer { get; init; }

    /// <summary>
    /// Include images in search results
    /// </summary>
    public bool IncludeImages { get; init; }
}
