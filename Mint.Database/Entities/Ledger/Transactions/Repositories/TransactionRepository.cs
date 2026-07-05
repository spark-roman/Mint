using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions.Dto;

namespace Mint.Database.Entities.Ledger.Transactions.Repositories;

/// <summary>
/// Repository for transactions
/// </summary>
/// <param name="transactionCreateMapper">Mapper for creating transaction</param>
/// <param name="transactionMapper">Mapper for transaction entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class TransactionRepository(
    IDbEntityMapper<TransactionCreateDto, TransactionEntity> transactionCreateMapper,
    IDbEntityMapper<TransactionEntity, TransactionDto> transactionMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : ITransactionRepository
{
    private readonly IDbEntityMapper<TransactionCreateDto, TransactionEntity> _transactionCreateMapper = transactionCreateMapper ?? throw new ArgumentNullException(nameof(transactionCreateMapper));

    private readonly IDbEntityMapper<TransactionEntity, TransactionDto> _transactionMapper = transactionMapper ?? throw new ArgumentNullException(nameof(transactionMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    /// <inheritdoc/>
    public async Task<long> CreateTransactionAsync(TransactionCreateDto transaction, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = _transactionCreateMapper.Map(transaction);

        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            IDbContextTransaction? dbTransaction = null;
            if (!context.Database.IsInMemory())
            {
                dbTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
            }

            try
            {
                var firstId = Math.Min(entity.DebetAccountId, entity.CreditAccountId);
                var secondId = Math.Max(entity.DebetAccountId, entity.CreditAccountId);

                Dictionary<long, AccountEntity> accounts;

                if (context.Database.IsInMemory())
                {
                    accounts = await context.Accounts
                        .Where(a => a.Id == firstId || a.Id == secondId)
                        .ToDictionaryAsync(a => a.Id, cancellationToken);
                }
                else
                {
                    accounts = await context.Accounts
                        .FromSqlRaw("SELECT * FROM \"accounts\" WHERE \"id\" IN ({0}, {1}) FOR UPDATE", firstId, secondId)
                        .ToDictionaryAsync(a => a.Id, cancellationToken);
                }

                if (!accounts.TryGetValue(entity.DebetAccountId, out var debetAccount))
                {
                    throw new InvalidOperationException($"Debet account not found: {entity.DebetAccountId}");
                }

                if (!accounts.TryGetValue(entity.CreditAccountId, out var creditAccount))
                {
                    throw new InvalidOperationException($"Credit account not found: {entity.CreditAccountId}");
                }

                if (debetAccount.Balance < entity.Amount)
                {
                    throw new InvalidOperationException($"Debet account balance is not enough for transaction: {entity.DebetAccountId}");
                }

                if (debetAccount.Status != AccountStatus.Active)
                {
                    throw new InvalidOperationException($"Debet account is not active: {entity.DebetAccountId}");
                }

                if (creditAccount.Status != AccountStatus.Active)
                {
                    throw new InvalidOperationException($"Credit account is not active: {entity.CreditAccountId}");
                }

                debetAccount.Balance -= entity.Amount;
                creditAccount.Balance += entity.Amount;

                await context.Transactions.AddAsync(entity, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                if (dbTransaction is not null)
                {
                    await dbTransaction.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                if (dbTransaction is not null)
                {
                    await dbTransaction.RollbackAsync(cancellationToken);
                }
                throw;
            }

            return entity.Id;
        });
    }

    /// <inheritdoc/>
    public async Task<TransactionDto?> GetTransactionByIdAsync(long transactionId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var transaction = await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId, cancellationToken);

        return transaction is null ? null : _transactionMapper.Map(transaction);
    }

    /// <inheritdoc/>
    public async Task<List<TransactionDto>?> GetTransactionsByAccountIdAsync(long accountId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entities = await context.Transactions
            .Where(t => t.CreditAccountId == accountId || t.DebetAccountId == accountId)
            .ToListAsync(cancellationToken);

        return entities.Select(_transactionMapper.Map).ToList();
    }
}
