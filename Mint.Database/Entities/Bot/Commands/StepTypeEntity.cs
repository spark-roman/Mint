using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mint.Database.Entities.Bot.Commands;

/// <summary>
/// Step type entity representing a type of step in a scenario
/// </summary>
[Table("step_types")]
public class StepTypeEntity
{
    /// <summary>
    /// Step type ID
    /// </summary>
    [Key]
    [Column("id")]
    public short Id { get; set; }

    /// <summary>
    /// Step type name
    /// </summary>
    [Required]
    [Column("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Step type description
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Steps of this type
    /// </summary>
    public virtual ICollection<StepEntity> Steps { get; init; } = new List<StepEntity>();
}
