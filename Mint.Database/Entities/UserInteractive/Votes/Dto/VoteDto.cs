namespace Mint.Database.Entities.UserInteractive.Votes.Dto;

/// <summary>
/// DTO for vote
/// </summary>
public record VoteDto
{
    /// <summary>
    /// Duel ID
    /// </summary>
    public long DuelId { get; init; }

    /// <summary>
    /// Account ID
    /// </summary>
    public long AccountId { get; init; }

    /// <summary>
    /// Option chosen
    /// </summary>
    public required string OptionChosen { get; init; }

    /// <summary>
    /// Bet amount
    /// </summary>
    public decimal BetAmount { get; init; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
