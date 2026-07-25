using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.News.RSS;

namespace Mint.Database.Entities.News;

/// <summary>
/// Represents a news item fetched from an RSS source.
/// </summary>
[Table("news")]
public class NewsEntity
{
    /// <summary>
    /// Unique identifier of the news item.
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Reference to the RSS source.
    /// </summary>
    [Column("rss_source_id")]
    public long? RssSourceId { get; set; }

    /// <summary>
    /// Title of the news item.
    /// </summary>
    [Required]
    [Column("title")]
    public required string Title { get; set; }

    /// <summary>
    /// URL of the news item.
    /// </summary>
    [Required]
    [Column("link")]
    public required string Link { get; set; }

    /// <summary>
    /// Description of the news item.
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Full content of the news item.
    /// </summary>
    [Column("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Author of the news item.
    /// </summary>
    [Column("author")]
    public string? Author { get; set; }

    /// <summary>
    /// Category code of the news item.
    /// </summary>
    [Column("category_code")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// Publication date of the news item.
    /// </summary>
    [Required]
    [Column("published_at")]
    public DateTimeOffset PublishedAt { get; set; }

    /// <summary>
    /// Indicates whether the news item has been processed for duel generation.
    /// </summary>
    [Column("is_processed")]
    public bool IsProcessed { get; set; } = false;

    /// <summary>
    /// Timestamp when the news item was processed.
    /// </summary>
    [Column("processed_at")]
    public DateTimeOffset? ProcessedAt { get; set; }

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// RSS source of the news item.
    /// </summary>
    public virtual RssSourceEntity? RssSource { get; set; }
}
