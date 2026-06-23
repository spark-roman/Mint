using Mint.Common.Contracts.Accounts;

namespace Mint.Database.Entities.Ledger.Accounts.Dto;

/// <summary>
/// DTO for creating an account
/// </summary>
public record AccountCreateDto
{
    /// <summary>
    /// External user Id (e.g., Telegram Id)
    /// </summary>
    public long ExternalUserId { get; set; }

    /// <summary>
    /// User Id
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// Auth system type
    /// </summary>
    public byte SystemType { get; init; }

    /// <summary>
    /// Initial balance
    /// </summary>
    public decimal Balance { get; init; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Account status
    /// </summary>
    public AccountStatus Status { get; init; } = AccountStatus.Active;
}
