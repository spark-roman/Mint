using Mint.Database.Entities.Ledger.Accounts.Dto;

namespace Mint.Database.Entities.Ledger.Accounts;

/// <summary>
/// Repository interface for working with user accounts
/// </summary>
public interface IAccountRepository
{
    /// <summary>
    /// Create a new user account
    /// </summary>
    /// <param name="account">DTO entity for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created account</returns>
    Task<long> CreateAccountAsync(AccountCreateDto account, CancellationToken cancellationToken);

    /// <summary>
    /// Get account by ID
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Account DTO or null if not found</returns>
    Task<AccountDto?> GetAccountByIdAsync(long accountId, CancellationToken cancellationToken);

    /// <summary>
    /// Get user account by external user ID and system type
    /// </summary>
    /// <param name="externalUserId">External user ID (e.g., Telegram ID)</param>
    /// <param name="systemType">Auth system type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of accounts</returns>
    Task<List<AccountDto>?> GetAccountsByExternalUserIdAsync(long externalUserId, byte systemType, CancellationToken cancellationToken);

    /// <summary>
    /// Update account balance
    /// </summary>
    /// <param name="dto">Update balance DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if account found and updated, otherwise false</returns>
    Task<bool> UpdateBalanceAsync(AccountUpdateBalanceDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Delete account by ID (soft delete - sets status to Deleted)
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>true if account found and deleted, otherwise false</returns>
    Task<bool> DeleteAccountAsync(long accountId, CancellationToken cancellationToken);
}
