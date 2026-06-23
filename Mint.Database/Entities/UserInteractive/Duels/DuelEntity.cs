using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Duel category (e.g. #ТехноИИ, #Мемы)
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("category")]
    public required string Category { get; set; }

    /// <summary>
    /// Duel question
    /// </summary>
    [Required]
    [StringLength(500)]
    [Column("question")]
    public required string Question { get; set; }

    /// <summary>
    /// Description of the AI-generated info event
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
    /// Votes for this duel
    /// </summary>
    public virtual ICollection<VoteEntity> Votes { get; init; } = new List<VoteEntity>();
}
