using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database.Entities.Ledger.Transactions;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes;

namespace Mint.Database.Entities.System.Payouts;

/// <summary>
/// Represents a payout made to a user for a winning vote in a duel.
/// </summary>
[Table("payouts")]
public class PayoutEntity
{
    /// <summary>
    /// Unique payout identifier.
    /// </summary>
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// Identifier of the duel.
    /// </summary>
    [Required]
    [Column("duel_id")]
    public long DuelId { get; set; }

    /// <summary>
    /// Identifier of the account receiving the payout.
    /// </summary>
    [Required]
    [Column("account_id")]
    public long CreditAccountId { get; set; }

    /// <summary>
    /// Payout amount.
    /// </summary>
    [Required]
    [Column("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Status of the payout.
    /// </summary>
    [Required]
    [Column("status")]
    public PayoutStatus Status { get; set; }

    /// <summary>
    /// Identifier of the transaction associated with this payout.
    /// Null until the payout has been executed by the payout execution job.
    /// </summary>
    [Column("transaction_id")]
    public long? TransactionId { get; set; }

    /// <summary>
    /// Debit account identifier captured when the payout plan was generated.
    /// Used to build the transaction at the execution phase.
    /// </summary>
    [Column("debit_account_id")]
    public long DebitAccountId { get; set; }

    /// <summary>
    /// Bonus type identifier captured when the payout plan was generated.
    /// Used to build the transaction at the execution phase.
    /// </summary>
    [Column("bonus_type_id")]
    public BonusType BonusType { get; set; } = BonusType.None;

    /// <summary>
    /// Transaction description captured when the payout plan was generated.
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Timestamp when the payout was processed.
    /// Null while the payout is only a plan that has not been executed yet.
    /// </summary>
    [Column("processed_at")]
    public DateTimeOffset? ProcessedAt { get; set; }

    /// <summary>
    /// Timestamp when the payout was created.
    /// </summary>
    [Column("planned_at")]
    public DateTimeOffset PlannedAt { get; set; }

    /// <summary>
    /// Identifier of the winning vote.
    /// </summary>
    [Required]
    [Column("vote_id")]
    public long VoteId { get; set; }

    /// <summary>
    /// The vote this payout belongs to.
    /// </summary>
    [ForeignKey(nameof(VoteId))]
    public VoteEntity Vote { get; set; } = null!;

    /// <summary>
    /// The duel this payout belongs to.
    /// </summary>
    [ForeignKey(nameof(DuelId))]
    public DuelEntity Duel { get; set; } = null!;

    /// <summary>
    /// The transaction associated with this payout.
    /// </summary>
    [ForeignKey(nameof(TransactionId))]
    public TransactionEntity? Transaction { get; set; }

    /// <summary>
    /// The reason for the failure of the payout.
    /// </summary>
    [Column("failure_reason")]
    public string? FailureReason { get; set; } 

    /// <summary>
    /// Number of times the payout has been retried.
    /// </summary>
    public int RetryCount { get; set; }
}

