using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;

namespace Mint.Database.Entities.UserInteractive.Bonuses.Mappers;

/// <summary>
/// Mapper for user bonus stats entity
/// </summary>
public class DbUserBonusStatsMapper : IDbEntityMapper<UserBonusStatsEntity, UserBonusStatsDto>
{
    /// <inheritdoc/>
    public UserBonusStatsDto Map(UserBonusStatsEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new UserBonusStatsDto
        {
            Id = entity.Id,
            InternalUserId = entity.UserId,
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
