using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Repositories;

/// <summary>
/// Repository for duels
/// </summary>
/// <param name="duelCreateMapper">Mapper for creating duel</param>
/// <param name="duelMapper">Mapper for duel entity</param>
/// <param name="duelOptionMapper">Mapper for duel option entity</param>
/// <param name="dbContextFactory">Database context factory</param>
/// <param name="timeProvider">Time provider</param>
public class DuelRepository(
    IDbEntityMapper<DuelCreateDto, DuelEntity> duelCreateMapper,
    IDbEntityMapper<DuelEntity, DuelDto> duelMapper,
    IDbEntityMapper<DuelOptionEntity, DuelOptionDto> duelOptionMapper,
    IDbContextFactory<MintDbContext> dbContextFactory,
    TimeProvider timeProvider) : IDuelRepository
{
    private readonly IDbEntityMapper<DuelCreateDto, DuelEntity> _duelCreateMapper = duelCreateMapper ?? throw new ArgumentNullException(nameof(duelCreateMapper));

    private readonly IDbEntityMapper<DuelEntity, DuelDto> _duelMapper = duelMapper ?? throw new ArgumentNullException(nameof(duelMapper));

    private readonly IDbEntityMapper<DuelOptionEntity, DuelOptionDto> _optionMapper = duelOptionMapper ?? throw new ArgumentNullException(nameof(duelOptionMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc/>
    public async Task<long> CreateDuelAsync(DuelCreateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var entity = _duelCreateMapper.Map(dto);

        await context.Duels.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task<DuelDto?> GetDuelByIdAsync(long duelId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var duel = await context.Duels
            .Include(d => d.Options)
            .FirstOrDefaultAsync(d => d.Id == duelId, cancellationToken);

        return duel is null ? null : _duelMapper.Map(duel);
    }

    /// <inheritdoc/>
    public async Task<List<DuelDto>?> GetActiveDuelsForCloseAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var now = _timeProvider.GetUtcNow();

        var entities = await context.Duels
            .Include(d => d.Options)
            .Where(d => d.Status == DuelStatus.Active && d.ExpiresAt < now)
            .OrderByDescending(d => d.Id)
            .ToListAsync(cancellationToken);

        return entities.Select(_duelMapper.Map).ToList();
    }

    /// <inheritdoc/>
    public async Task<DuelDto?> GetFirstAvailableDuelAsync(int categoryId, long accountId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var now = _timeProvider.GetUtcNow();

        var duel = await context.Duels
            .Include(d => d.Options)
            .Include(d => d.Category)
            .Include(d => d.Votes)
            .FirstOrDefaultAsync(d => d.CategoryId == categoryId
                && d.Status == DuelStatus.Active
                && d.ExpiresAt > now
                && !d.Votes.Any(v => v.AccountId == accountId), cancellationToken);

        return duel is null ? null : _duelMapper.Map(duel);
    }

    /// <inheritdoc/>
    public async Task<DuelOptionDto?> GetOptionByIdAsync(long optionId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = await context.DuelOptions
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == optionId, cancellationToken);

        return entity != null ? _optionMapper.Map(entity) : null;
    }

    /// <inheritdoc/>
    public async Task CloseDuelAsync(long duelId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var duel = await context.Duels.FirstOrDefaultAsync(d => d.Id == duelId, cancellationToken);

        if (duel is null)
        {
            throw new InvalidOperationException("Duel not found");
        }

        duel.Status = DuelStatus.Closed;

        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<int> PublishDuelsAsync(DateTimeOffset expiresAt, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var duels = await context.Duels.Where(d => d.Status == DuelStatus.Planned).ToListAsync(cancellationToken);

        if (duels is null || duels.Count == 0)
        {
            return 0;
        }

        foreach(var duel in duels)
        {
            duel.Status = DuelStatus.Closed;
            duel.ExpiresAt = expiresAt;
        }

        await context.SaveChangesAsync(cancellationToken);

        return duels.Count;
    }
}
