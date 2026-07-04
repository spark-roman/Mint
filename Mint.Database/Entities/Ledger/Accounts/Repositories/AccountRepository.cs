using AdvApplication.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Ledger.Accounts.Dto;

namespace Mint.Database.Entities.Ledger.Accounts.Repositories;

/// <summary>
/// Repository for accounts
/// </summary>
/// <param name="userRepository">User repository</param>
/// <param name="accountCreateMapper">Mapper for creating account</param>
/// <param name="accountMapper">Mapper for account entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class AccountRepository(
    IUserRepository userRepository,
    IDbEntityMapper<AccountCreateDto, AccountEntity> accountCreateMapper,
    IDbEntityMapper<AccountEntity, AccountDto> accountMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : IAccountRepository
{
    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly IDbEntityMapper<AccountCreateDto, AccountEntity> _accountCreateMapper = accountCreateMapper ?? throw new ArgumentNullException(nameof(accountCreateMapper));

    private readonly IDbEntityMapper<AccountEntity, AccountDto> _accountMapper = accountMapper ?? throw new ArgumentNullException(nameof(accountMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    /// <inheritdoc/>
    public async Task<long> CreateAccountAsync(AccountCreateDto account, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(account);

        var user = await _userRepository.GetUserAsync(account.ExternalUserId, account.SystemType, cancellationToken);

        if (user is null)
        {
            throw new InvalidOperationException($"User not found: {account.ExternalUserId}");
        }
        else
        {
            account.UserId = user.Id;
        }
        
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = _accountCreateMapper.Map(account);

        await context.Accounts.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task<AccountDto?> GetAccountByIdAsync(long accountId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        return account is null ? null : _accountMapper.Map(account);
    }

    /// <inheritdoc/>
    public async Task<List<AccountDto>?> GetAccountsByExternalUserIdAsync(long externalUserId, byte systemType, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = await context.Accounts
            .Include(a => a.User)
            .Where(a => a.User.ExternalUserId == externalUserId && a.User.SystemType == systemType && a.Status == AccountStatus.Active)
            .ToListAsync(cancellationToken);

        return entities.Select(_accountMapper.Map).ToList();
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateBalanceAsync(AccountUpdateBalanceDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == dto.AccountId, cancellationToken);

        if (account is null)
        {
            return false;
        }

        account.Balance = dto.NewBalance;
        account.LastTransactionDate = dto.LastTransactionDate;
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAccountAsync(long accountId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var account = await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);

        if (account is null)
        {
            return false;
        }

        account.Status = AccountStatus.Deleted;
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc/>
    public async Task<decimal> GetUserBalanceAsync(long externalUserId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var balnce = context.Users
            .Include(u => u.Account)
            .Select(u => u.Account.Balance)
            .FirstOrDefault();

        return balnce;
    }
}
