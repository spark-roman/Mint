using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.News.RSS;

/// <summary>
/// Represents an RSS source entity.
/// </summary>
[Table("rss_sources")]
public class RssSourceEntity
{
    /// <summary>
    /// Unique identifier of the RSS source.
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Name of the RSS source.
    /// </summary>
    [Required]
    [Column("name")]
    public required string Name { get; set; }

    /// <summary>
    /// URL of the RSS feed.
    /// </summary>
    [Required]
    [Column("url")]
    public required Uri Url { get; set; }

    /// <summary>
    /// Category code of the RSS source.
    /// </summary>
    [Column("category_code")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// Indicates whether the RSS source is active.
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Priority of the RSS source (higher = more important).
    /// </summary>
    [Column("priority")]
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Language of the RSS source (ru, en, etc.).
    /// </summary>
    [Column("language")]
    [MaxLength(10)]
    public string Language { get; set; } = "en";

    /// <summary>
    /// Creation timestamp.
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Collection of news items from this source.
    /// </summary>
    public virtual ICollection<NewsEntity> News { get; init; } = [];
}
