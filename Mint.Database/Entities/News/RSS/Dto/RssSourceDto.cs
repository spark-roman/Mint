namespace Mint.Database.Entities.News.RSS.Dto;

/// <summary>
/// DTO for RssSource.
/// </summary>
public sealed record RssSourceDto
{
    /// <summary>
    /// ID of the RSS source.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Name of the RSS source.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// URL of the RSS feed.
    /// </summary>
    public required Uri Url { get; init; }

    /// <summary>
    /// Category code of the RSS source.
    /// </summary>
    public string? CategoryCode { get; init; }

    /// <summary>
    /// Whether the RSS source is active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Priority of the RSS source.
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Language of the RSS source.
    /// </summary>
    public string? Language { get; init; }
}
