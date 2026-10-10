using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Payouts;

namespace Mint.Database.Entities.System.Payouts.Dto;

/// <summary>
/// Data transfer object for creating a payout.
/// </summary>
/// <remarks>
/// A payout is created as a plan: <see cref="TransactionId"/> and <see cref="ProcessedAt"/>
/// are empty until the payout execution job turns the plan into a transaction.
/// </remarks>
public sealed record PayoutCreateDto
{
    /// <summary>Identifier of the winning vote.</summary>
    public required long VoteId { get; init; }

    /// <summary>Identifier of the duel.</summary>
    public required long DuelId { get; init; }

    /// <summary>Identifier of the account receiving the payout.</summary>
    public required long AccountId { get; init; }

    /// <summary>Payout amount.</summary>
    public required decimal Amount { get; init; }

    /// <summary>Debit account to charge when the payout is executed.</summary>
    public long DebitAccountId { get; init; }

    /// <summary>Bonus type to book when the payout is executed.</summary>
    public BonusType BonusType { get; init; }

    /// <summary>Description to attach to the transaction.</summary>
    public string? Description { get; init; }

    /// <summary>Identifier of the transaction. Null for a payout plan.</summary>
    public long? TransactionId { get; init; }

    /// <summary>Status of the payout.</summary>
    public PayoutStatus Status { get; set; }

    /// <summary>Timestamp when the payout was processed. Null for a payout plan.</summary>
    public DateTimeOffset? ProcessedAt { get; init; }
}
