using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.Users;

namespace Mint.Database.Entities.UserInteractive.Stats.Mappers;

/// <summary>
/// Mapper for user stats entity
/// </summary>
public class DbUserStatsMapper : IDbUserStatsMapper
{
    /// <inheritdoc/>
    public UserStatsDto Map(UserStatsEntity statsEntity, UserEntity userEntity)
    {
        ArgumentNullException.ThrowIfNull(statsEntity);
        ArgumentNullException.ThrowIfNull(userEntity);

        return new UserStatsDto
        {
            Id = statsEntity.Id,
            UserId = statsEntity.UserId,
            ExternalUserId = userEntity.ExternalUserId,
            RankPoints = statsEntity.RankPoints,
            TotalWins = statsEntity.TotalWins,
            TotalLosses = statsEntity.TotalLosses,
            ReferralCount = statsEntity.ReferralCount,
            UpdatedAt = statsEntity.UpdatedAt,
            UserName = userEntity.UserName ?? userEntity.FirstName ?? "Аноним",
        };
    }
}
