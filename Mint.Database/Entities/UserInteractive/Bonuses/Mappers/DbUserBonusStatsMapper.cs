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
            CurrentDailyStreak = entity.CurrentDailyStreak,
            LastDailyClaimedAt = entity.LastDailyClaimedAt,
            NextDailyAvailableAt = entity.NextDailyAvailableAt,
            TotalReferralBonusesClaimed = entity.TotalReferralBonusesClaimed,
            LastRatingBonusClaimedAt = entity.LastRatingBonusClaimedAt
        };
    }
}
