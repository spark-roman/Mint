namespace Mint.Database.Entities.News.RSS.Dto;

/// <summary>
/// Data transfer object for creating a new RSS source.
/// </summary>
public sealed record RssSourceCreateDto
{
    /// <summary>
    /// Name of the RSS source.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// URL of the RSS source.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// Code of the category to which this RSS source belongs.
    /// </summary>
    public string? CategoryCode { get; init; }

    /// <summary>
    /// Priority of the RSS source (higher = more important).
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Language of the RSS source (ru, en, etc.).
    /// </summary>
    public string Language { get; init; } = "en";
}
