using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.System.Settings;

/// <summary>
/// System setting entity for storing configurable parameters.
/// </summary>
[Table("system_settings")]
public class SystemSettingEntity
{
    /// <summary>
    /// Setting unique identifier.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Setting key (e.g., "StartBonus", "DailyBonus").
    /// </summary>
    [Required]
    [Column("key")]
    [MaxLength(100)]
    public required string Key { get; set; }

    /// <summary>
    /// Setting value stored as JSON string for flexibility.
    /// </summary>
    [Required]
    [Column("value")]
    public required string Value { get; set; }

    /// <summary>
    /// Setting description.
    /// </summary>
    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Last update timestamp.
    /// </summary>
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
