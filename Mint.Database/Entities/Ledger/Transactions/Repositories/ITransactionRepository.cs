using Mint.Database.Entities.Ledger.Transactions.Dto;

namespace Mint.Database.Entities.Ledger.Transactions.Repositories;

/// <summary>
/// Repository interface for working with transactions
/// </summary>
public interface ITransactionRepository
{
    /// <summary>
    /// Create a new transaction
    /// </summary>
    /// <param name="transaction">DTO entity for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created transaction</returns>
    Task<long> CreateTransactionAsync(TransactionCreateDto transaction, CancellationToken cancellationToken);

    /// <summary>
    /// Get transaction by ID
    /// </summary>
    /// <param name="transactionId">Transaction ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Transaction DTO or null if not found</returns>
    Task<TransactionDto?> GetTransactionByIdAsync(long transactionId, CancellationToken cancellationToken);

    /// <summary>
    /// Get transactions by account ID
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of transactions</returns>
    Task<List<TransactionDto>?> GetTransactionsByAccountIdAsync(long accountId, CancellationToken cancellationToken);
}
