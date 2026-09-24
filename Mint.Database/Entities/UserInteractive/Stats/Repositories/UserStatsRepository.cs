using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Mappers;

namespace Mint.Database.Entities.UserInteractive.Stats.Repositories;

/// <summary>
/// Repository for user stats
/// </summary>
/// <param name="statsCreateMapper">Mapper for creating stats</param>
/// <param name="statsUpdateMapper">Mapper for updating stats</param>
/// <param name="statsMapper">Mapper for stats entity</param>
/// <param name="dbUserStatsMapper">Mapper for leaderboard stats</param>
/// <param name="timeProvider">Time provider</param>
/// <param name="dbContextFactory">Database context factory</param>
public class UserStatsRepository(
    IDbEntityMapper<UserStatsCreateDto, UserStatsEntity> statsCreateMapper,
    IDbEntityMapper<UserStatsUpdateDto, UserStatsEntity> statsUpdateMapper,
    IDbEntityMapper<UserStatsEntity, UserStatsDto> statsMapper,
    IDbUserStatsMapper dbUserStatsMapper,
    TimeProvider timeProvider,
    IDbContextFactory<MintDbContext> dbContextFactory) : IUserStatsRepository
{
    private readonly IDbEntityMapper<UserStatsCreateDto, UserStatsEntity> _statsCreateMapper = statsCreateMapper ?? throw new ArgumentNullException(nameof(statsCreateMapper));

    private readonly IDbEntityMapper<UserStatsUpdateDto, UserStatsEntity> _statsUpdateMapper = statsUpdateMapper ?? throw new ArgumentNullException(nameof(statsUpdateMapper));

    private readonly IDbEntityMapper<UserStatsEntity, UserStatsDto> _statsMapper = statsMapper ?? throw new ArgumentNullException(nameof(statsMapper));

    private readonly IDbUserStatsMapper _dbUserStatsMapper = dbUserStatsMapper ?? throw new ArgumentNullException(nameof(dbUserStatsMapper));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

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
    public async Task<UserStatsDto?> GetStatsByUserIdAsync(long externaUserId, byte systemType, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = context.Users
            .Where(u => u.ExternalUserId == externaUserId && u.SystemType == systemType)
            .Include(u => u.Stats)
            .Select(u => u.Stats)
            .FirstOrDefault();

        return stats is null ? null : _statsMapper.Map(stats);
    }

    /// <inheritdoc/>
    public async Task<UserStatsDto?> GetStatsByAccountIdAsync(long accountId, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = context.Accounts
            .Where(a => a.Id == accountId)
            .Include(a => a.User)
            .ThenInclude(u => u.Stats)
            .Select(a => a.User.Stats)
            .FirstOrDefault();

        return stats is null ? null : _statsMapper.Map(stats);
    }

    /// <inheritdoc/>
    public async Task<List<UserStatsDto>> GetTopStatsByUserIdAsync(int top, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = await context.Users
            .AsNoTracking()
            .Include(u => u.Stats)
            .Select(u => new { u.Stats, User = u })
            .Where(u => u.Stats != null && u.User != null)
            .OrderByDescending(u => u.Stats.RankPoints)
            .Take(top)
            .Select(s =>_dbUserStatsMapper.Map(s.Stats, s.User))
            .ToListAsync(cancellationToken);

        return stats;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateStatsAsync(long externalUserId, byte systemType, UserStatsUpdateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = context.Users
            .Where(u => u.ExternalUserId == externalUserId && u.SystemType == systemType)
            .Include(u => u.Stats)
            .Select(u => u.Stats)
            .FirstOrDefault();

        if (stats is null)
        {
            return false;
        }

        ApplyUpdate(stats, dto);

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateStatsByAccountIdAsync(long accountId, UserStatsUpdateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (context.Database.IsInMemory())
        {
            var inMemoryStats = context.Accounts
                .Where(a => a.Id == accountId)
                .Include(a => a.User)
                .ThenInclude(u => u.Stats)
                .Select(a => a.User.Stats)
                .FirstOrDefault();

            if (inMemoryStats is null)
            {
                return false;
            }

            ApplyUpdate(inMemoryStats, dto);

            await context.SaveChangesAsync(cancellationToken);
            return true;
        }

        var updated = await context.UserStats
            .Where(s => context.Accounts.Any(a => a.Id == accountId && a.UserId == s.UserId))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(s => s.RankPoints, dto.RankPoints)
                .SetProperty(s => s.TotalWins, dto.TotalWins)
                .SetProperty(s => s.TotalLosses, dto.TotalLosses)
                .SetProperty(s => s.TotalDraws, dto.TotalDraws)
                .SetProperty(s => s.ReferralCount, dto.ReferralCount)
                .SetProperty(s => s.InvitedByUserId, dto.InvitedByUserId)
                .SetProperty(s => s.UpdatedAt, _timeProvider.GetUtcNow()),
                cancellationToken);

        return updated > 0;
    }

    /// <summary>
    /// Applies the update DTO values to the stats entity.
    /// </summary>
    /// <param name="stats">Stats entity to update.</param>
    /// <param name="dto">Update DTO.</param>
    private void ApplyUpdate(UserStatsEntity stats, UserStatsUpdateDto dto)
    {
        var updatedEntity = _statsUpdateMapper.Map(dto);
        stats.RankPoints = updatedEntity.RankPoints;
        stats.TotalWins = updatedEntity.TotalWins;
        stats.TotalLosses = updatedEntity.TotalLosses;
        stats.TotalDraws = updatedEntity.TotalDraws;
        stats.ReferralCount = updatedEntity.ReferralCount;
        stats.InvitedByUserId = updatedEntity.InvitedByUserId;
        stats.UpdatedAt = updatedEntity.UpdatedAt;
    }

    /// <inheritdoc/>
    public async Task<int> GetUserRankByPointsAsync(decimal rankPoints, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var higherCount = await context.UserStats
            .AsNoTracking()
            .CountAsync(s => s.RankPoints > rankPoints, cancellationToken);

        return higherCount + 1;
    }
}
