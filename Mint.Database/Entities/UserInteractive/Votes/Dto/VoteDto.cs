namespace Mint.Database.Entities.UserInteractive.Votes.Dto;

/// <summary>
/// DTO for vote
/// </summary>
public record VoteDto
{
    /// <summary>
    /// Account ID
    /// </summary>
    public long AccountId { get; init; }

    /// <summary>
    /// Duel ID
    /// </summary>
    public long DuelId { get; init; }

    /// <summary>
    /// Chosen option ID
    /// </summary>
    public long ChosenOptionId { get; init; }

    /// <summary>
    /// Bet amount
    /// </summary>
    public decimal BetAmount { get; init; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
