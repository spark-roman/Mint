using Mint.Common.Contracts.UserInteractive.Bonuses;

namespace Mint.Database.Entities.Ledger.Transactions.Dto;

/// <summary>
/// DTO for creating a transaction
/// </summary>
public record TransactionCreateDto
{
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
    /// Type of bonus
    /// </summary>
    public BonusType BonusType { get; init; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
