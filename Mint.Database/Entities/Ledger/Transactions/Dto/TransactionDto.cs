using Mint.Common.Contracts.UserInteractive.Bonuses;

namespace Mint.Database.Entities.Ledger.Transactions.Dto;

/// <summary>
/// DTO for transaction
/// </summary>
public record TransactionDto
{
    /// <summary>
    /// Transaction ID
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Account ID
    /// </summary>
    public long AccountId { get; init; }

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
