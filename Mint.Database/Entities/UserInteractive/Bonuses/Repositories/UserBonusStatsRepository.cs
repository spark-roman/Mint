using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;

namespace Mint.Database.Entities.UserInteractive.Bonuses.Repositories;

/// <summary>
/// Repository for user bonus stats
/// </summary>
/// <param name="statsCreateMapper">Mapper for creating stats</param>
/// <param name="statsMapper">Mapper for stats entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class UserBonusStatsRepository(
    IDbEntityMapper<UserBonusStatsCreateDto, UserBonusStatsEntity> statsCreateMapper,
    IDbEntityMapper<UserBonusStatsEntity, UserBonusStatsDto> statsMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : IUserBonusStatsRepository
{
    private readonly IDbEntityMapper<UserBonusStatsCreateDto, UserBonusStatsEntity> _statsCreateMapper = statsCreateMapper ?? throw new ArgumentNullException(nameof(statsCreateMapper));

    private readonly IDbEntityMapper<UserBonusStatsEntity, UserBonusStatsDto> _statsMapper = statsMapper ?? throw new ArgumentNullException(nameof(statsMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    /// <inheritdoc/>
    public async Task<long> CreateStatsAsync(UserBonusStatsCreateDto dto, CancellationToken cancellationToken)
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

        await context.UserBonusStats.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    /// <inheritdoc/>
    public async Task<UserBonusStatsDto?> GetStatsByUserIdAsync(long externalUserId, byte systemType, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = context.UserBonusStats
            .Include(ubs => ubs.User)
            .FirstOrDefault(ubs => ubs.User.ExternalUserId == externalUserId && ubs.User.SystemType == systemType);

        return stats is null ? null : _statsMapper.Map(stats);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateStatsAsync(UserBonusStatsUpdateDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var stats = context.Users
            .Where(u => u.ExternalUserId == dto.ExternalUserId)
            .Include(u => u.BonusStats)
            .Select(u => u.BonusStats)
            .FirstOrDefault();

        if (stats is null)
        {
            return false;
        }

        stats.IsStartBonusClaimed = dto.IsStartBonusClaimed;
        stats.TotalStartBonusesClaimed = dto.TotalStartBonusesClaimed;
        stats.CurrentDailyStreak = dto.CurrentDailyStreak;
        stats.TotalStreakBonusesClaimed = dto.TotalStreakBonusesClaimed;
        stats.LastStreakClaimedAt = dto.LastStreakClaimedAt;
        stats.TotalDailyBonusesClaimed = dto.TotalDailyBonusesClaimed;
        stats.LastDailyClaimedAt = dto.LastDailyClaimedAt;
        stats.NextDailyAvailableAt = dto.NextDailyAvailableAt;
        stats.TotalReferralBonusesClaimed = dto.TotalReferralBonusesClaimed;
        stats.TotalRankBonusClaimed = dto.TotalRankBonusClaimed;
        stats.LastRankBonusClaimedAt = dto.LastRankBonusClaimedAt;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
