using System.Collections.ObjectModel;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database.Entities.System.Payouts.Dto;

namespace Mint.Database.Entities.System.Payouts.Repositories;

/// <inheritdoc cref="IPayoutRepository"/>
public sealed class PayoutRepository(
    IDbContextFactory<MintDbContext> dbContextFactory,
    IDbEntityMapper<PayoutEntity, PayoutDto> mapper,
    IDbEntityMapper<PayoutCreateDto, PayoutEntity> createMapper) : IPayoutRepository
{
    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory
        ?? throw new ArgumentNullException(nameof(dbContextFactory));

    private readonly IDbEntityMapper<PayoutEntity, PayoutDto> _mapper = mapper
        ?? throw new ArgumentNullException(nameof(mapper));

    private readonly IDbEntityMapper<PayoutCreateDto, PayoutEntity> _createMapper = createMapper
        ?? throw new ArgumentNullException(nameof(createMapper));

    /// <inheritdoc />
    public async Task<PayoutDto> CreateAsync(PayoutCreateDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var entity = _createMapper.Map(dto);
        entity.PlannedAt = DateTimeOffset.UtcNow;
        entity.Status = PayoutStatus.Pending;

        await context.Payouts.AddAsync(entity, ct);
        await context.SaveChangesAsync(ct);

        return _mapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task<List<PayoutDto>> CreateManyAsync(ReadOnlyCollection<PayoutCreateDto> dtos, CancellationToken ct)
    {
        if (dtos == null || dtos.Count == 0)
            return [];

        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        var entities = dtos.Select(dto =>
        {
            var entity = _createMapper.Map(dto);
            entity.PlannedAt = DateTimeOffset.UtcNow;
            entity.Status = PayoutStatus.Pending;
            return entity;
        }).ToList();

        await context.Payouts.AddRangeAsync(entities, ct);
        await context.SaveChangesAsync(ct);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<PayoutDto> UpdateAsync(PayoutUpdateDto dto, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(dto);

        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        var entity = await context.Payouts
            .FirstOrDefaultAsync(p => p.Id == dto.Id, ct);

        if (entity == null)
            throw new InvalidOperationException($"Payout {dto.Id} not found");

        entity.Status = dto.Status;
        if (dto.TransactionId.HasValue)
            entity.TransactionId = dto.TransactionId;
        if (dto.ProcessedAt.HasValue)
            entity.ProcessedAt = dto.ProcessedAt;

        context.Payouts.Update(entity);
        await context.SaveChangesAsync(ct);

        return _mapper.Map(entity);
    }

    /// <inheritdoc />
    public async Task<List<PayoutDto>> UpdateManyAsync(ReadOnlyCollection<PayoutUpdateDto> dtos, CancellationToken ct)
    {
        if (dtos == null || dtos.Count == 0)
            return [];

        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        var ids = dtos.Select(d => d.Id).ToList();
        var entities = await context.Payouts
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(ct);

        var dtoMap = dtos.ToDictionary(d => d.Id);

        foreach (var entity in entities)
        {
            if (dtoMap.TryGetValue(entity.Id, out var dto))
            {
                entity.Status = dto.Status;
                if (dto.TransactionId.HasValue)
                    entity.TransactionId = dto.TransactionId;
            }
        }

        context.Payouts.UpdateRange(entities);
        await context.SaveChangesAsync(ct);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<PayoutDto?> GetByIdAsync(long id, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var entity = await context.Payouts
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        return entity != null ? _mapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<List<PayoutDto>> GetByDuelIdAsync(long duelId, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var entities = await context.Payouts
            .AsNoTracking()
            .Where(p => p.DuelId == duelId)
            .OrderByDescending(p => p.PlannedAt)
            .ToListAsync(ct);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<List<PayoutDto>> GetByAccountIdAsync(long accountId, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var entities = await context.Payouts
            .AsNoTracking()
            .Where(p => p.CreditAccountId == accountId)
            .OrderByDescending(p => p.PlannedAt)
            .ToListAsync(ct);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<List<PayoutDto>> GetPendingPayoutsAsync(int limit, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        var entities = await context.Payouts
            .AsNoTracking()
            .Where(p => p.Status == PayoutStatus.Pending)
            .OrderBy(p => p.PlannedAt)
            .Take(limit)
            .ToListAsync(ct);

        return entities.Select(_mapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<List<PayoutDto>> ClaimPendingPayoutsAsync(int limit, CancellationToken ct)
    {
        if (limit <= 0)
        {
            return [];
        }

        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TimeSpan.FromSeconds(30)
        };

        using var scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        if (context.Database.IsInMemory())
        {
            var payoutsForUpdate = await context.Payouts
                .Where(p => p.Status == PayoutStatus.Pending)
                .Take(limit)
                .ToListAsync(ct);

            foreach (var payout in payoutsForUpdate)
            {
                payout.Status = PayoutStatus.Processing;
            }

            await context.SaveChangesAsync(ct);
        }
        else
        {
            var updated = await context.Payouts
                .Where(p => p.Status == PayoutStatus.Pending)
                .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.Status, PayoutStatus.Processing), ct);
        }

        var claimed = context.Payouts.Where(p => p.Status == PayoutStatus.Processing).Select(_mapper.Map).ToList();

        scope.Complete();

        return claimed;
    }

    /// <inheritdoc />
    public async Task<bool> TryCompleteAsync(long id, long? transactionId, DateTimeOffset processedAt, CancellationToken ct)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);

        if (context.Database.IsInMemory())
        {
            var payout = await context.Payouts
                .FirstOrDefaultAsync(p => p.Id == id && p.Status == PayoutStatus.Processing, ct);

            if (payout is null)
            {
                return false;
            }

            payout.Status = PayoutStatus.Completed;
            payout.TransactionId = transactionId;
            payout.ProcessedAt = processedAt;

            await context.SaveChangesAsync(ct);
            return true;
        }

        var updated = await context.Payouts
            .Where(p => p.Id == id && p.Status == PayoutStatus.Processing)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(p => p.Status, PayoutStatus.Completed)
                .SetProperty(p => p.TransactionId, transactionId)
                .SetProperty(p => p.ProcessedAt, processedAt),
                ct);

        return updated > 0;
    }
}
