namespace Mint.Database.Entities.News.RSS.Dto;

/// <summary>
/// Data transfer object for RSS source.
/// </summary>
public sealed record RssSourceDto
{
    /// <summary>
    /// Unique identifier.
    /// </summary>
    public required long Id { get; init; }

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
    /// Is the RSS source active.
    /// </summary>
    public bool IsActive { get; init; }

    /// <summary>
    /// Priority of the RSS source.
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Language of the RSS source.
    /// </summary>
    public string Language { get; init; } = "en";

    /// <summary>
    /// Date and time when the RSS source was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Date and time when the RSS source was updated.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}
