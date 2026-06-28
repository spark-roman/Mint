using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.Database.Entities.System.Dto;

/// <summary>
/// DTO for AI prompt
/// </summary>
public record AiPromptDto
{
    /// <summary>
    /// Prompt ID
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// System prompt template for AI generation
    /// </summary>
    public required string SystemPromptTemplate { get; init; }

    /// <summary>
    /// User prompt template for AI generation
    /// </summary>
    public required string UserPromptTemplate { get; init; }

    /// <summary>
    /// Temperature parameter for API (creativity: from 0.0 to 1.0)
    /// </summary>
    public float Temperature { get; init; } = 0.6f;

    /// <summary>
    /// Maximum number of duels generated at one time
    /// </summary>
    public int MaxDuelsPerRun { get; init; } = 3;

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }

    /// <summary>
    /// Active categories for AI generation
    /// </summary>
    public required IEnumerable<CategoryDto> Categories { get; init; } = [];
}
