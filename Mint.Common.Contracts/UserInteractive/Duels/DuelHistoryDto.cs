namespace Mint.Common.Contracts.UserInteractive.Duels;

/// <summary>
/// Result type of a duel vote
/// </summary>
public enum DuelVoteResultType
{
    /// <summary>
    /// User has not voted in this duel
    /// </summary>
    NoVote = 0,

    /// <summary>
    /// User lost the duel
    /// </summary>
    Loss = 1,

    /// <summary>
    /// Draw (refund)
    /// </summary>
    Draw = 2,

    /// <summary>
    /// User won the duel
    /// </summary>
    Win = 3
}

/// <summary>
/// DTO for a single duel vote in history
/// </summary>
public sealed record DuelHistoryDto
{
    /// <summary>
    /// Duel ID
    /// </summary>
    public required long DuelId { get; init; }

    /// <summary>
    /// Name of the duel category
    /// </summary>
    public required string CategoryName { get; init; } = string.Empty;

    /// <summary>
    /// Result type (Win/Loss/Draw/NoVote)
    /// </summary>
    public required DuelVoteResultType Result { get; init; }

    /// <summary>
    /// Bet amount the user placed
    /// </summary>
    public required decimal BetAmount { get; init; }

    /// <summary>
    /// Payout amount received (0 if lost, bet amount if draw, profit if won)
    /// </summary>
    public required decimal PayoutAmount { get; init; }

    /// <summary>
    /// When the vote was placed
    /// </summary>
    public required DateTimeOffset VotedAt { get; init; }
}
