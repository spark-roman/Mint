namespace Mint.Database.Entities.UserInteractive.Stats.Dto;

/// <summary>
/// Projection for duel history vote query
/// </summary>
public sealed record DuelHistoryProjectionDto
{
    /// <summary>
    /// Vote id
    /// </summary>
    public required long Id { get; init; }

    /// <summary>
    /// Duel id
    /// </summary>
    public required long DuelId { get; init; }

    /// <summary>
    /// Category name
    /// </summary>
    public required string CategoryName { get; init; } = string.Empty;

    /// <summary>
    /// Bet amount
    /// </summary>
    public required decimal BetAmount { get; init; }

    /// <summary>
    /// Created at
    /// </summary>
    public required DateTimeOffset CreatedAt { get; init; }
}
