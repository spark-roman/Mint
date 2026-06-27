using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.UserInteractive.UserCategories;

namespace Mint.Database.Entities.System;

/// <summary>
/// AI generation settings and system prompts
/// </summary>
[Table("ai_prompts")]
public class AiPromptEntity
{
    /// <summary>
    /// Entity ID
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Core security rules, response format, and limitations (Core Prompt)
    /// </summary>
    [Required]
    [Column("system_prompt_template")]
    public string SystemPromptTemplate { get; set; } = string.Empty;

    /// <summary>
    /// User prompt template for AI generation
    /// </summary>
    [Required]
    [Column("user_prompt_template")]
    public string UserPromptTemplate { get; set; } = string.Empty;

    /// <summary>
    /// Temperature parameter for API (creativity: from 0.0 to 1.0)
    /// </summary>
    [Column("temperature")]
    public float Temperature { get; set; } = 0.6f;

    /// <summary>
    /// Maximum number of duels generated at one time
    /// </summary>
    [Column("max_duels_per_run")]
    public int MaxDuelsPerRun { get; set; } = 3;

    /// <summary>
    /// Last update timestamp
    /// </summary>
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Active categories for AI generation
    /// </summary>
    public virtual ICollection<CategoryEntity> Categories { get; } = [];
}
