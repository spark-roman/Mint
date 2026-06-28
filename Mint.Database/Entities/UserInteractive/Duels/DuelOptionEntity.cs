using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.UserInteractive.Votes;

namespace Mint.Database.Entities.UserInteractive.Duels;

/// <summary>
/// Duel option entity representing a possible answer in a duel
/// </summary>
[Table("duel_options")]
public class DuelOptionEntity
{
    /// <summary>
    /// Option ID
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Duel ID
    /// </summary>
    [Required]
    [Column("duel_id")]
    public long DuelId { get; set; }

    /// <summary>
    /// Parent duel
    /// </summary>
    public DuelEntity Duel { get; set; } = null!;

    /// <summary>
    /// Text displayed on the button
    /// </summary>
    [Required]
    [StringLength(200)]
    [Column("option_text")]
    public required string OptionText { get; set; }

    /// <summary>
    /// Short code for the option (e.g. "up", "hold")
    /// </summary>
    [Required]
    [StringLength(10)]
    [Column("option_code")]
    public required string OptionCode { get; set; }

    /// <summary>
    /// Votes for this option
    /// </summary>
    public virtual ICollection<VoteEntity> Votes { get; init; } = new List<VoteEntity>();
}
