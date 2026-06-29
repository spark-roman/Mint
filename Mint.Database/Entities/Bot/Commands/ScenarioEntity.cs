using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.Bot.Commands;

/// <summary>
/// Scenario entity representing a dialog scenario
/// </summary>
[Table("scenarios")]
public class ScenarioEntity
{
    /// <summary>
    /// Scenario ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Scenario name
    /// </summary>
    [Required]
    [Column("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Whether the scenario is active
    /// </summary>
    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Scenario creation timestamp
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Steps for this scenario
    /// </summary>
    public virtual ICollection<StepEntity> Steps { get; init; } = new List<StepEntity>();

    /// <summary>
    /// User sessions for this scenario
    /// </summary>
    public virtual ICollection<UserSessionEntity> UserSessions { get; init; } = new List<UserSessionEntity>();
}
