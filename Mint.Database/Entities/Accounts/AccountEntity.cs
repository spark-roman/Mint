using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.Users;

namespace Mint.Database.Entities.Accounts;

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
    public long Id { get; set; }

    /// <summary>
    /// User ID that owns the account
    /// </summary>
    [Required]
    public long UserId { get; set; }

    /// <summary>
    /// User entity
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public virtual UserEntity User { get; set; } = null!;

    /// <summary>
    /// Current balance
    /// </summary>
    public decimal Balance { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Last transaction date
    /// </summary>
    public DateTimeOffset? LastTransactionDate { get; set; }
}
