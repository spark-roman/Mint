namespace Mint.Database.Entities.UserInteractive.Duels.Dto;

/// <summary>
/// DTO for duel
/// </summary>
public record DuelDto
{
    /// <summary>
    /// Duel ID
    /// </summary>
    public long Id { get; init; }

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

    /// <summary>
    /// Whether the duel is closed
    /// </summary>
    public bool IsClosed { get; init; }
}
