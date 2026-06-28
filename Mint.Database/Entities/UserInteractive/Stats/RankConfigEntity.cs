using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.UserInteractive.Stats;

/// <summary>
/// Rank configuration entity
/// </summary>
[Table("ranks_config")]
public class RankConfigEntity
{
    /// <summary>
    /// Rank config ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Rank code (e.g., "newbie", "analyst", "oracle")
    /// </summary>
    [Required]
    [StringLength(50)]
    [Column("code")]
    public required string Code { get; set; }

    /// <summary>
    /// Rank name (e.g., "Оракул")
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Rank emoji (e.g., "👁️")
    /// </summary>
    [Required]
    [StringLength(10)]
    [Column("emoji")]
    public required string Emoji { get; set; }

    /// <summary>
    /// Minimum points required to obtain this rank
    /// </summary>
    [Required]
    [Column("min_points")]
    public int MinPoints { get; set; }
}
