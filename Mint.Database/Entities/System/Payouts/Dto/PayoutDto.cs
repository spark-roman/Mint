using Mint.Common.Contracts.UserInteractive.Payouts;

namespace Mint.Database.Entities.System.Payouts.Dto;

/// <summary>
/// Data transfer object for a payout.
/// </summary>
public sealed record PayoutDto
{
    /// <summary>Unique identifier of the payout.</summary>
    public required long Id { get; init; }

    /// <summary>Identifier of the winning vote.</summary>
    public required long VoteId { get; init; }

    /// <summary>Identifier of the duel.</summary>
    public required long DuelId { get; init; }

    /// <summary>Identifier of the account receiving the payout.</summary>
    public required long AccountId { get; init; }

    /// <summary>Payout amount.</summary>
    public required decimal Amount { get; init; }

    /// <summary>Status of the payout.</summary>
    public required PayoutStatus Status { get; init; }

    /// <summary>Identifier of the associated transaction.</summary>
    public long? TransactionId { get; init; }

    /// <summary>Timestamp when the payout was processed.</summary>
    public required DateTimeOffset ProcessedAt { get; init; }

    /// <summary>Timestamp when the payout was created.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
}
