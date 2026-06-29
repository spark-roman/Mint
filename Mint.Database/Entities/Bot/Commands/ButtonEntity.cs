using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.Bot.Commands;

/// <summary>
/// Button entity representing a button on a step
/// </summary>
[Table("buttons")]
public class ButtonEntity
{
    /// <summary>
    /// Button ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Step ID (foreign key)
    /// </summary>
    [Required]
    [Column("step_id")]
    public long StepId { get; set; }

    /// <summary>
    /// Parent step
    /// </summary>
    public virtual StepEntity ParentStep { get; set; } = null!;

    /// <summary>
    /// Button order number within the step
    /// </summary>
    [Required]
    [Column("order_num")]
    public short OrderNum { get; set; }

    /// <summary>
    /// Button caption text
    /// </summary>
    [Required]
    [Column("caption")]
    public required string Caption { get; set; }

    /// <summary>
    /// Button action
    /// </summary>
    [Required]
    [Column("action")]
    public required string Action { get; set; }

    /// <summary>
    /// Next step ID (foreign key, nullable)
    /// </summary>
    [Column("next_step_id")]
    public long? NextStepId { get; set; }

    /// <summary>
    /// Next step
    /// </summary>
    public virtual StepEntity? NextStep { get; set; }
}
