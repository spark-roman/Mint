using Mint.Common.Contracts.Transactions;

namespace Mint.Database.Entities.Transactions.Dto;

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
    /// Transaction type
    /// </summary>
    public TransactionType TransactionType { get; init; }

    /// <summary>
    /// Transaction description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
