using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.News.RSS;

/// <summary>
/// Represents an RSS source.
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
    [StringLength(200)]
    public required string Name { get; set; }

    /// <summary>
    /// URL of the RSS feed.
    /// </summary>
    [Required]
    [Column("url")]
    [StringLength(500)]
    public required Uri Url { get; set; }

    /// <summary>
    /// Category code of the RSS source.
    /// </summary>
    [Column("category_code")]
    [StringLength(50)]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// Whether the RSS source is active.
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Priority of the RSS source (higher = more important).
    /// </summary>
    [Column("priority")]
    public int Priority { get; set; } = 0;

    /// <summary>
    /// Language of the RSS source.
    /// </summary>
    [Column("language")]
    [StringLength(10)]
    public string? Language { get; set; }

    /// <summary>
    /// News items from this RSS source.
    /// </summary>
    public virtual ICollection<NewsEntity> News { get; } = new List<NewsEntity>();
}
