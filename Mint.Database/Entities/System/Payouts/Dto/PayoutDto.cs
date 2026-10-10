using Mint.Common.Contracts.UserInteractive.Bonuses;
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
    public required long CreditAccountId { get; init; }

    /// <summary>Payout amount.</summary>
    public required decimal Amount { get; init; }

    /// <summary>Status of the payout.</summary>
    public required PayoutStatus Status { get; init; }

    /// <summary>
    /// Debit account to charge when the payout is executed.
    /// Captured by the payout generation job, null when the payout needs no debit.
    /// </summary>
    public long DebitAccountId { get; init; }

    /// <summary>
    /// Stored <see cref="Mint.Common.Contracts.UserInteractive.Bonuses.BonusType"/> value to book
    /// when the payout is executed. Captured by the payout generation job.
    /// </summary>
    public BonusType BonusType { get; init; }

    /// <summary>Description to attach to the transaction. Captured by the payout generation job.</summary>
    public string? Description { get; init; }

    /// <summary>Identifier of the associated transaction. Null while the payout is only a plan.</summary>
    public long? TransactionId { get; init; }

    /// <summary>
    /// Timestamp when the payout was executed.
    /// Null while the payout is scheduled and has not been executed yet.
    /// </summary>
    public DateTimeOffset? ProcessedAt { get; init; }

    /// <summary>Timestamp when the payout was created.</summary>
    public required DateTimeOffset CreatedAt { get; init; }
}
