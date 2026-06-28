using Mint.Common.Contracts.Ledger.Accounts;

namespace Mint.Database.Entities.Ledger.Accounts.Dto;

/// <summary>
/// DTO for user account
/// </summary>
public record AccountDto
{
    /// <summary>
    /// Account ID
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// User Id
    /// </summary>
    public long UserId { get; init; }

    /// <summary>
    /// Current balance
    /// </summary>
    public decimal Balance { get; init; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Last transaction date
    /// </summary>
    public DateTimeOffset? LastTransactionDate { get; init; }

    /// <summary>
    /// Account status
    /// </summary>
    public AccountStatus Status { get; init; }
}
