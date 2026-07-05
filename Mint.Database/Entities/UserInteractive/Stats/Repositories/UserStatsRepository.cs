using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Repositories;

/// <summary>
/// Repository for user stats
/// </summary>
/// <param name="statsCreateMapper">Mapper for creating stats</param>
/// <param name="statsUpdateMapper">Mapper for updating stats</param>
/// <param name="statsMapper">Mapper for stats entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class UserStatsRepository(
    IDbEntityMapper<UserStatsCreateDto, UserStatsEntity> statsCreateMapper,
    IDbEntityMapper<UserStatsUpdateDto, UserStatsEntity> statsUpdateMapper,
    IDbEntityMapper<UserStatsEntity, UserStatsDto> statsMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : IUserStatsRepository
{
    private readonly IDbEntityMapper<UserStatsCreateDto, UserStatsEntity> _statsCreateMapper = statsCreateMapper ?? throw new ArgumentNullException(nameof(statsCreateMapper));

    private readonly IDbEntityMapper<UserStatsUpdateDto, UserStatsEntity> _statsUpdateMapper = statsUpdateMapper ?? throw new ArgumentNullException(nameof(statsUpdateMapper));

    private readonly IDbEntityMapper<UserStatsEntity, UserStatsDto> _statsMapper = statsMapper ?? throw new ArgumentNullException(nameof(statsMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    /// <inheritdoc/>
    public async Task<long> CreateStatsAsync(UserStatsCreateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var userEntity = await context.Users.FirstOrDefaultAsync(u => u.ExternalUserId == dto.ExternalUserId, cancellationToken);

        if (userEntity is null)
        {
            throw new InvalidOperationException("User not found");
        }
        else
        {
            dto.InternalUserId = userEntity.Id;
        }

        var entity = _statsCreateMapper.Map(dto);

        await context.UserStats.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task<UserStatsDto?> GetStatsByUserIdAsync(long userId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = context.Users
            .Where(u => u.ExternalUserId == userId)
            .Include(u => u.Stats)
            .Select(u => u.Stats)
            .FirstOrDefault();

        return stats is null ? null : _statsMapper.Map(stats);
    }

    /// <inheritdoc/>
    public async Task<List<UserStatsDto>> GetTopStatsByUserIdAsync(int top, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = context.Users
            .AsNoTracking()
            .Include(u => u.Stats)
            .Select(u => new { Stats = u.Stats, u.ExternalUserId })
            .OrderBy(u => u.Stats.RankPoints)
            .Take(top)
            .ToList();

        return stats.Select(s =>
        {
            var dto = _statsMapper.Map(s.Stats);
            dto.ExternalUserId = s.ExternalUserId;
            return dto;
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateStatsAsync(long userId, UserStatsUpdateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = context.Users
            .Where(u => u.ExternalUserId == userId)
            .Include(u => u.Stats)
            .Select(u => u.Stats)
            .FirstOrDefault();

        if (stats is null)
        {
            return false;
        }
            
        var updatedEntity = _statsUpdateMapper.Map(dto);
        stats.RankPoints = updatedEntity.RankPoints;
        stats.TotalWins = updatedEntity.TotalWins;
        stats.TotalLosses = updatedEntity.TotalLosses;
        stats.UpdatedAt = updatedEntity.UpdatedAt;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
