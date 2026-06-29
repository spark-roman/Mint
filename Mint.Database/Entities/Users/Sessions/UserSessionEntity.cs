using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.Users.Sessions;

/// <summary>
/// User session entity representing a user's interaction with a scenario
/// </summary>
[Table("user_sessions")]
public class UserSessionEntity
{
    /// <summary>
    /// Session ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// User ID
    /// </summary>
    [Required]
    [Column("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// Scenario ID (foreign key)
    /// </summary>
    [Required]
    [Column("scenario_id")]
    public long ScenarioId { get; set; }

    /// <summary>
    /// Scenario
    /// </summary>
    public virtual ScenarioEntity Scenario { get; set; } = null!;

    /// <summary>
    /// Current step ID (foreign key)
    /// </summary>
    [Required]
    [Column("current_step_id")]
    public long CurrentStepId { get; set; }

    /// <summary>
    /// Current step
    /// </summary>
    public virtual StepEntity CurrentStep { get; set; } = null!;

    /// <summary>
    /// Session data (JSONB)
    /// </summary>
    [Column("data")]
    public required string Data { get; set; } = "{}";

    /// <summary>
    /// Session start timestamp
    /// </summary>
    [Column("started_at")]
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>
    /// Session completion timestamp
    /// </summary>
    [Column("completed_at")]
    public DateTimeOffset? CompletedAt { get; set; }
}
