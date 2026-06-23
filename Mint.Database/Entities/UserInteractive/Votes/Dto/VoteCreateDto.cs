namespace Mint.Database.Entities.UserInteractive.Votes.Dto;

/// <summary>
/// DTO for creating a vote
/// </summary>
public record VoteCreateDto
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
    /// Option chosen ('A' or 'B')
    /// </summary>
    public required string OptionChosen { get; init; }

    /// <summary>
    /// Bet amount in coins
    /// </summary>
    public decimal BetAmount { get; init; }
}
