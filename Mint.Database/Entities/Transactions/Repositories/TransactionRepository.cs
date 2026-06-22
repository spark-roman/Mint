using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Transactions.Dto;

namespace Mint.Database.Entities.Transactions.Repositories;

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

        await context.Transactions.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
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
            .Where(t => t.AccountId == accountId)
            .ToListAsync(cancellationToken);

        return entities.Select(_transactionMapper.Map).ToList();
    }
}
