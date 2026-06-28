using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Common.Contracts.UserInteractive;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.UserInteractive.Votes;

namespace Mint.Database.Entities.UserInteractive.Duels;

/// <summary>
/// Duel entity representing a question duel between users
/// </summary>
[Table("duels")]
public class DuelEntity
{
    /// <summary>
    /// Duel ID
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Category ID
    /// </summary>
    [Required]
    [Column("category_id")]
    public int CategoryId { get; set; }

    /// <summary>
    /// Parent category
    /// </summary>
    public CategoryEntity Category { get; set; } = null!;

    /// <summary>
    /// Duel type
    /// </summary>
    [Required]
    [Column("duel_type")]
    public DuelType DuelType { get; set; }

    /// <summary>
    /// Duel question
    /// </summary>
    [Required]
    [StringLength(500)]
    [Column("question")]
    public required string Question { get; set; }

    /// <summary>
    /// Description of the duel
    /// </summary>
    [Required]
    [StringLength(2000)]
    [Column("description")]
    public required string Description { get; set; }

    /// <summary>
    /// Duel expiration date
    /// </summary>
    [Required]
    [Column("expires_at")]
    public DateTimeOffset ExpiresAt { get; set; }

    /// <summary>
    /// Whether the duel is closed
    /// </summary>
    [Column("is_closed")]
    public bool IsClosed { get; set; } = false;

    /// <summary>
    /// Available options for this duel
    /// </summary>
    public virtual ICollection<DuelOptionEntity> Options { get; init; } = new List<DuelOptionEntity>();

    /// <summary>
    /// Votes for this duel
    /// </summary>
    public virtual ICollection<VoteEntity> Votes { get; init; } = new List<VoteEntity>();
}
