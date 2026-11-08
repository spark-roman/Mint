using System.Collections.ObjectModel;
using Gnews;
using Mint.App.Services.System.News;
using Mint.Database.Entities.News.Dto;

namespace Mint.App.Services.System.News.Google;

/// <summary>
/// Settings for GNews API
/// </summary>
public record GNewsSettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "GNewsSettings";

    /// <summary>
    /// GNews API key from https://gnews.io/
    /// </summary>
    public required string ApiKey { get; init; } = string.Empty;

    /// <summary>
    /// Number of search results to return
    /// </summary>
    public int MaxResults { get; init; } = 10;

    /// <summary>
    /// Number of days back to search for news
    /// </summary>
    public int DaysBack { get; init; } = 30;

    /// <summary>
    /// Language filter (e.g., "ru", "en")
    /// </summary>
    public string? Language { get; init; }

    /// <summary>
    /// Country filter (e.g., "ru", "us")
    /// </summary>
    public string? Country { get; init; }
}
