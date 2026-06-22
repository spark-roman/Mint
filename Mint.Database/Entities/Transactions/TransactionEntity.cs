using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.Accounts;

namespace Mint.Database.Entities.Transactions;

/// <summary>
/// Transaction entity
/// </summary>
public class TransactionEntity
{
    /// <summary>
    /// Transaction ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// Account ID that owns the transaction
    /// </summary>
    [Required]
    [Column("account_id")]
    public long AccountId { get; set; }

    /// <summary>
    /// Account entity
    /// </summary>
    [ForeignKey(nameof(AccountId))]
    public virtual AccountEntity Account { get; set; } = null!;

    /// <summary>
    /// Transaction amount (+ income, - expense)
    /// </summary>
    [Column("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Transaction type
    /// </summary>
    [Column("transaction_type")]
    public Common.Contracts.Transactions.TransactionType TransactionType { get; set; }

    /// <summary>
    /// Transaction description
    /// </summary>
    [Column("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
