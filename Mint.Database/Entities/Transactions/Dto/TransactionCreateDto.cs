namespace Mint.Database.Entities.Transactions.Dto;

/// <summary>
/// DTO for creating a transaction
/// </summary>
public record TransactionCreateDto
{
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
    /// Creation date
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
