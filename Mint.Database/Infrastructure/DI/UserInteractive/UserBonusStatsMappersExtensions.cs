using Mint.Common.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.Bonuses;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;
using Mint.Database.Entities.UserInteractive.Bonuses.Mappers;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for user bonus stats mappers
/// </summary>
public static class UserBonusStatsMappersExtensions
{
    /// <summary>
    /// Register user bonus stats mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterUserBonusStatsMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<UserBonusStatsCreateDto, UserBonusStatsEntity>, DbUserBonusStatsCreateMapper>();
        services.AddSingleton<IDbEntityMapper<UserBonusStatsUpdateDto, UserBonusStatsEntity>, DbUserBonusStatsUpdateMapper>();
        services.AddSingleton<IDbEntityMapper<UserBonusStatsEntity, UserBonusStatsDto>, DbUserBonusStatsMapper>();
    }
}
