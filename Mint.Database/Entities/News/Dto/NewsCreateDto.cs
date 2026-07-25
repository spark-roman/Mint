namespace Mint.Database.Entities.News.Dto;

/// <summary>
/// Data transfer object for creating a news item.
/// </summary>
public sealed record NewsCreateDto
{
    /// <summary>
    /// RSS source id
    /// </summary>
    public long? RssSourceId { get; init; }

    /// <summary>
    /// Title of the news item
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// URL to the news item
    /// </summary>
    public required string Link { get; init; }

    /// <summary>
    /// Description of the news item
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Content of the news item
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Author of the news item
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Category code of the news item
    /// </summary>
    public string? CategoryCode { get; init; }

    /// <summary>
    /// Date when the news was published
    /// </summary>
    public required DateTimeOffset PublishedAt { get; init; }
}
