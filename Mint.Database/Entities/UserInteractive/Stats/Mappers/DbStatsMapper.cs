using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Mappers;

/// <summary>
/// Mapper for user stats entity
/// </summary>
public class DbStatsMapper : IDbEntityMapper<UserStatsEntity, UserStatsDto>
{
    /// <inheritdoc/>
    public UserStatsDto Map(UserStatsEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new UserStatsDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            RankPoints = entity.RankPoints,
            TotalWins = entity.TotalWins,
            TotalLosses = entity.TotalLosses,
            ReferralCount = entity.ReferralCount,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
