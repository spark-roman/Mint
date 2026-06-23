namespace Mint.Database.Entities.UserInteractive.Duels.Dto;

/// <summary>
/// DTO for creating a duel
/// </summary>
public record DuelCreateDto
{
    /// <summary>
    /// Duel category
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Duel question
    /// </summary>
    public required string Question { get; init; }

    /// <summary>
    /// Description of the info event
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Duel expiration date
    /// </summary>
    public DateTimeOffset ExpiresAt { get; init; }
}
