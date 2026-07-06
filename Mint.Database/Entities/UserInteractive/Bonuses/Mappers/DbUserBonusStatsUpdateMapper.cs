using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;

namespace Mint.Database.Entities.UserInteractive.Bonuses.Mappers;

/// <summary>
/// Mapper for updating user bonus stats entity
/// </summary>
public class DbUserBonusStatsUpdateMapper : IDbEntityMapper<UserBonusStatsUpdateDto, UserBonusStatsEntity>
{
    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="timeProvider">Time provider</param>
    public DbUserBonusStatsUpdateMapper(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    private readonly TimeProvider _timeProvider;

    /// <inheritdoc/>
    public UserBonusStatsEntity Map(UserBonusStatsUpdateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new UserBonusStatsEntity
        {
            UserId = entity.InternalUserId,
            IsStartBonusClaimed = entity.IsStartBonusClaimed,
            TotalStartBonusesClaimed = entity.TotalStartBonusesClaimed,
            CurrentDailyStreak = entity.CurrentDailyStreak,
            TotalStreakBonusesClaimed = entity.TotalStreakBonusesClaimed,
            LastStreakClaimedAt = entity.LastStreakClaimedAt,
            TotalDailyBonusesClaimed = entity.TotalDailyBonusesClaimed,
            LastDailyClaimedAt = entity.LastDailyClaimedAt,
            NextDailyAvailableAt = entity.NextDailyAvailableAt,
            TotalReferralBonusesClaimed = entity.TotalReferralBonusesClaimed,
            TotalRankBonusClaimed = entity.TotalRankBonusClaimed,
            LastRankBonusClaimedAt = entity.LastRankBonusClaimedAt
        };
    }
}
