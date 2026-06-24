using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Repositories;

/// <summary>
/// Repository for duels
/// </summary>
/// <param name="duelCreateMapper">Mapper for creating duel</param>
/// <param name="duelMapper">Mapper for duel entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class DuelRepository(
    IDbEntityMapper<DuelCreateDto, DuelEntity> duelCreateMapper,
    IDbEntityMapper<DuelEntity, DuelDto> duelMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : IDuelRepository
{
    private readonly IDbEntityMapper<DuelCreateDto, DuelEntity> _duelCreateMapper = duelCreateMapper ?? throw new ArgumentNullException(nameof(duelCreateMapper));

    private readonly IDbEntityMapper<DuelEntity, DuelDto> _duelMapper = duelMapper ?? throw new ArgumentNullException(nameof(duelMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

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
    public async Task<List<DuelDto>?> GetActiveDuelsAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        var entities = await context.Duels
            .Include(d => d.Options)
            .Where(d => d.IsClosed == false && d.ExpiresAt > now)
            .OrderByDescending(d => d.Id)
            .ToListAsync(cancellationToken);

        return entities.Select(_duelMapper.Map).ToList();
    }
}
