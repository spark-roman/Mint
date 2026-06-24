namespace Mint.Database.Entities.System.Dto;

/// <summary>
/// DTO for creating AI prompt
/// </summary>
public record AiPromptCreateDto
{
    /// <summary>
    /// Core security rules, response format, and limitations
    /// </summary>
    public required string SystemCoreText { get; init; }

    /// <summary>
    /// Temperature parameter for API (creativity: from 0.0 to 1.0)
    /// </summary>
    public float Temperature { get; init; } = 0.6f;

    /// <summary>
    /// Maximum number of duels generated at one time
    /// </summary>
    public int MaxDuelsPerRun { get; init; } = 3;
}
