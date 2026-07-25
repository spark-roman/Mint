namespace Mint.Database.Entities.News.Dto;

/// <summary>
/// Data transfer object for fetching unprocessed news.
/// </summary>
public sealed record UnprocessedNewsQueryDto
{
    /// <summary>
    /// Number of news items to fetch.
    /// </summary>
    public int Limit { get; init; } = 50;

    /// <summary>
    /// Category code of the news item.
    /// </summary>
    public string? CategoryCode { get; init; }
}
