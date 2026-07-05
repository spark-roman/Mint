using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;

namespace Mint.Database.Entities.UserInteractive.Bonuses.Repositories;

/// <summary>
/// Repository for user bonus stats
/// </summary>
/// <param name="statsCreateMapper">Mapper for creating stats</param>
/// <param name="statsUpdateMapper">Mapper for updating stats</param>
/// <param name="statsMapper">Mapper for stats entity</param>
/// <param name="dbContextFactory">Database context factory</param>
public class UserBonusStatsRepository(
    IDbEntityMapper<UserBonusStatsCreateDto, UserBonusStatsEntity> statsCreateMapper,
    IDbEntityMapper<UserBonusStatsUpdateDto, UserBonusStatsEntity> statsUpdateMapper,
    IDbEntityMapper<UserBonusStatsEntity, UserBonusStatsDto> statsMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : IUserBonusStatsRepository
{
    private readonly IDbEntityMapper<UserBonusStatsCreateDto, UserBonusStatsEntity> _statsCreateMapper = statsCreateMapper ?? throw new ArgumentNullException(nameof(statsCreateMapper));

    private readonly IDbEntityMapper<UserBonusStatsUpdateDto, UserBonusStatsEntity> _statsUpdateMapper = statsUpdateMapper ?? throw new ArgumentNullException(nameof(statsUpdateMapper));

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

        var stats = context.Users
            .Where(u => u.ExternalUserId == externalUserId && u.SystemType == systemType)
            .Include(u => u.BonusStats)
            .Select(u => u.BonusStats)
            .FirstOrDefault();

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

        var updatedEntity = _statsUpdateMapper.Map(dto);
        stats.IsStartBonusClaimed = updatedEntity.IsStartBonusClaimed;
        stats.CurrentDailyStreak = updatedEntity.CurrentDailyStreak;
        stats.LastDailyClaimedAt = updatedEntity.LastDailyClaimedAt;
        stats.NextDailyAvailableAt = updatedEntity.NextDailyAvailableAt;
        stats.TotalReferralBonusesClaimed = updatedEntity.TotalReferralBonusesClaimed;
        stats.LastRatingBonusClaimedAt = updatedEntity.LastRatingBonusClaimedAt;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
