using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database.Entities.Users;

namespace Mint.Database.Entities.Ledger.Accounts;

/// <summary>
/// User account entity
/// </summary>
public class AccountEntity
{
    /// <summary>
    /// Account ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// User ID that owns the account
    /// </summary>
    [Required]
    [Column("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// User entity
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual UserEntity User { get; set; } = null!;

    /// <summary>
    /// Current balance
    /// </summary>
    [Column("balance")]
    public decimal Balance { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last transaction date
    /// </summary>
    [Column("last_transaction_date")]
    public DateTimeOffset? LastTransactionDate { get; set; }

    /// <summary>
    /// Account status
    /// </summary>
    [Column("status")]
    public AccountStatus Status { get; set; }
}
