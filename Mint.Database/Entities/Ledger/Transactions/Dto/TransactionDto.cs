using Mint.Common.Contracts.UserInteractive.Bonuses;

namespace Mint.Database.Entities.Ledger.Transactions.Dto;

/// <summary>
/// DTO for transaction
/// </summary>
public record TransactionDto
{
    /// <summary>
    /// Transaction id
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Debit account id
    /// </summary>
    public long DebitAccountId { get; init; }

    /// <summary>
    /// Credit account id
    /// </summary>
    public long CreditAccountId { get; init; }

    /// <summary>
    /// Transaction amount
    /// </summary>
    public decimal Amount { get; init; }

    /// <summary>
    /// Transaction description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Bonus type
    /// </summary>
    public BonusType BounusType { get; init; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
