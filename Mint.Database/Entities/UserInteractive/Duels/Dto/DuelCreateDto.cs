using Mint.Common.Contracts.UserInteractive;

namespace Mint.Database.Entities.UserInteractive.Duels.Dto;

/// <summary>
/// DTO for creating a duel
/// </summary>
public record DuelCreateDto
{
    /// <summary>
    /// Category ID
    /// </summary>
    public required int CategoryId { get; init; }

    /// <summary>
    /// Duel type
    /// </summary>
    public DuelType DuelType { get; init; }

    /// <summary>
    /// Duel question
    /// </summary>
    public required string Question { get; init; }

    /// <summary>
    /// Description of the duel
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Duel expiration date
    /// </summary>
    public DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Options for the duel
    /// </summary>
    public required IEnumerable<DuelOptionCreateDto> Options { get; init; } = [];
}
