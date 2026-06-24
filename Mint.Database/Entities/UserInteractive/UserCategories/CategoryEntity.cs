using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.System;

namespace Mint.Database.Entities.UserInteractive.UserCategories;

/// <summary>
/// User category entity
/// </summary>
[Table("user_categories")]
public class CategoryEntity
{
    /// <summary>
    /// Category ID
    /// </summary>
    [Key]
    [Column("id")]
    public int Id { get; set; }

    /// <summary>
    /// AI Prompt ID
    /// </summary>
    [Column("ai_prompt_id")]
    public int? AiPromptId { get; set; }

    /// <summary>
    /// Parent AI prompt
    /// </summary>
    public AiPromptEntity? AiPrompt { get; set; }

    /// <summary>
    /// Category name
    /// </summary>
    [Required]
    [StringLength(100)]
    [Column("name")]
    public required string Name { get; set; }

    /// <summary>
    /// Category description
    /// </summary>
    [StringLength(500)]
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Whether AI will search news for this category. If false, the category is excluded from generation.
    /// </summary>
    [Column("is_active_for_ai")]
    public bool IsActiveForAI { get; set; } = true;

    /// <summary>
    /// AI hints for which specific topics to search (e.g., "TON, Bitcoin, Telegram Wallet, Ethereum, аирдропы")
    /// </summary>
    [StringLength(2000)]
    [Column("search_keywords")]
    public string? SearchKeywords { get; set; }
}
