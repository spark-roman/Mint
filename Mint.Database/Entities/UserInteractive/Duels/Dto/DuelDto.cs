using Mint.Common.Contracts.UserInteractive;

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
    /// Category ID
    /// </summary>
    public int CategoryId { get; init; }

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
    /// Whether the duel is closed
    /// </summary>
    public bool IsClosed { get; init; }

    /// <summary>
    /// Options for the duel
    /// </summary>
    public required IEnumerable<DuelOptionDto> Options { get; init; } = [];

    /// <summary>
    /// Category name
    /// </summary>
    public string CategoryName { get; init; } = string.Empty;
}
