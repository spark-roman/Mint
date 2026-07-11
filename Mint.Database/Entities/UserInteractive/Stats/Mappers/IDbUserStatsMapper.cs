using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.Users;

namespace Mint.Database.Entities.UserInteractive.Stats.Mappers;

/// <summary>
/// Mapper for user stats entity
/// </summary>
public interface IDbUserStatsMapper
{
    /// <summary>
    /// Map user stats entity to dto
    /// </summary>
    /// <param name="statsEntity">Stats entity</param>
    /// <param name="userEntity">User entity</param>
    /// <returns>User stats dto</returns>
    UserStatsDto Map(UserStatsEntity statsEntity, UserEntity userEntity);
}
