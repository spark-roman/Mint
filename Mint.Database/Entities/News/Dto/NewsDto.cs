namespace Mint.Database.Entities.News.Dto;

/// <summary>
/// Data transfer object for news item.
/// </summary>
public sealed record NewsDto
{
    /// <summary>
    /// Id of the news item.
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// RSS source id.
    /// </summary>
    public long? RssSourceId { get; init; }

    /// <summary>
    /// Title of the news item.
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// Url to the news item.
    /// </summary>
    public required string Link { get; init; }

    /// <summary>
    /// Description of the news item.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Content of the news item.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    /// Author of the news item.
    /// </summary>
    public string? Author { get; init; }

    /// <summary>
    /// Category code of the news item.
    /// </summary>
    public string? CategoryCode { get; init; }

    /// <summary>
    /// Date when the news was published.
    /// </summary>
    public required DateTimeOffset PublishedAt { get; init; }
    
    /// <summary>
    /// Indicates whether the news item has been processed.
    /// </summary>
    public bool IsProcessed { get; init; }

    /// <summary>
    /// Date when the news item was processed.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; init; }

    /// <summary>
    /// Date when the news item was created.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
