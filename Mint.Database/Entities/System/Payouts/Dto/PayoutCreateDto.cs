namespace Mint.Database.Entities.System.Payouts.Dto;

/// <summary>
/// Data transfer object for creating a payout.
/// </summary>
public sealed record PayoutCreateDto
{
    /// <summary>Identifier of the winning vote.</summary>
    public required long VoteId { get; init; }

    /// <summary>Identifier of the duel.</summary>
    public required long DuelId { get; init; }

    /// <summary>Identifier of the account receiving the payout.</summary>
    public required long AccountId { get; init; }

    /// <summary>Identifier of the transaction that was used to payout.</summary>
    public required long TransactionId { get; init; }

    /// <summary>Payout amount.</summary>
    public required decimal Amount { get; init; }

    /// <summary>Timestamp when the payout was processed.</summary>
    public required DateTimeOffset ProcessedAt { get; init; }
}
