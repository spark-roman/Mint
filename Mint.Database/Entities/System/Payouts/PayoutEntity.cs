using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database.Entities.Ledger.Accounts;
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
    public long AccountId { get; set; }

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
    /// </summary>
    [Column("transaction_id")]
    public long? TransactionId { get; set; }

    /// <summary>
    /// Timestamp when the payout was processed.
    /// </summary>
    [Required]
    [Column("processed_at")]
    public DateTimeOffset ProcessedAt { get; set; }

    /// <summary>
    /// Timestamp when the payout was created.
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

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
    /// The account receiving the payout.
    /// </summary>
    [ForeignKey(nameof(AccountId))]
    public AccountEntity Account { get; set; } = null!;

    /// <summary>
    /// The transaction associated with this payout.
    /// </summary>
    [ForeignKey(nameof(TransactionId))]
    public TransactionEntity? Transaction { get; set; }
}

