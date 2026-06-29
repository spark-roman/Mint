using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.Bot.Commands;

/// <summary>
/// Step entity representing a step in a scenario
/// </summary>
[Table("steps")]
public class StepEntity
{
    /// <summary>
    /// Step ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

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
    /// Step order number within the scenario
    /// </summary>
    [Required]
    [Column("order_num")]
    public short OrderNum { get; set; }

    /// <summary>
    /// Step type ID (foreign key)
    /// </summary>
    [Required]
    [Column("step_type_id")]
    public short StepTypeId { get; set; }

    /// <summary>
    /// Step type
    /// </summary>
    public virtual StepTypeEntity StepType { get; set; } = null!;

    /// <summary>
    /// Step message text
    /// </summary>
    [Required]
    [Column("message")]
    public required string Message { get; set; }

    /// <summary>
    /// Whether this is the final step in the scenario
    /// </summary>
    [Column("is_final")]
    public bool IsFinal { get; set; } = false;

    /// <summary>
    /// Step data (JSONB)
    /// </summary>
    [Column("data")]
    public string? Data { get; set; }

    /// <summary>
    /// Buttons for this step
    /// </summary>
    public virtual ICollection<ButtonEntity> Buttons { get; init; } = new List<ButtonEntity>();
}
