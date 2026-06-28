using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.UserInteractive.Bonuses;

/// <summary>
/// Bonus type entity.
/// </summary>
[Table("bonus_types")]
public class BonusTypeEntity
{
    /// <summary>
    /// Bonus type id.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// Bonus type code.
    /// </summary>
    [Column("code")]
    [StringLength(50)]
    public required string Code { get; set; }

    /// <summary>
    /// Bonus type name.
    /// </summary>
    [Column("name")]
    [StringLength(100)]
    public required string Name { get; set; }

    /// <summary>
    /// Bonus type description.
    /// </summary>
    [Column("description")]
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Is this bonus type active.
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Creation date.
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Update date.
    /// </summary>
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
