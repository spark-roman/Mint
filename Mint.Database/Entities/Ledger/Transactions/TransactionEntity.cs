using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Bonuses;

namespace Mint.Database.Entities.Ledger.Transactions;

/// <summary>
/// Transaction entity
/// </summary>
[Table("transactions")]
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
    /// Debet account id
    /// </summary>
    [Required]
    [Column("debet_account_id")]
    public long DebetAccountId { get; init; }

    /// <summary>
    /// Debet account entity
    /// </summary>
    [ForeignKey(nameof(DebetAccountId))]
    public virtual AccountEntity DebetAccount { get; set; } = null!;

    /// <summary>
    /// Credit account id
    /// </summary>
    [Required]
    [Column("creadit_account_id")]
    public long CreditAccountId { get; init; }

    /// <summary>
    /// Debet account entity
    /// </summary>
    [ForeignKey(nameof(CreditAccountId))]
    public virtual AccountEntity CreditAccount { get; set; } = null!;

    /// <summary>
    /// Type of bonus id
    /// </summary>
    [Required]
    [Column("bonus_type_id")]
    public int BonusTypeId { get; set; }

    /// <summary>
    /// Bonus type entity
    /// </summary>
    public BonusTypeEntity TransactionType { get; set; } = null!;

    /// <summary>
    /// Transaction amount (+ income, - expense)
    /// </summary>
    [Column("amount")]
    public decimal Amount { get; set; }

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
