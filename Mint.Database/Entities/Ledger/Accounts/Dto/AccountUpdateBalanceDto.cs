namespace Mint.Database.Entities.Ledger.Accounts.Dto;

/// <summary>
/// DTO for updating account balance
/// </summary>
public record AccountUpdateBalanceDto
{
    /// <summary>
    /// Account ID
    /// </summary>
    public long AccountId { get; init; }

    /// <summary>
    /// New balance value
    /// </summary>
    public decimal NewBalance { get; init; }

    /// <summary>
    /// Last transaction date
    /// </summary>
    public DateTimeOffset LastTransactionDate { get; init; }
}
