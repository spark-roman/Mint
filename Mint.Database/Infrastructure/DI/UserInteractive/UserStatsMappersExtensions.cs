using Mint.Common.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Mappers;
using Mint.Database.Entities.UserInteractive.Stats;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for user stats mappers
/// </summary>
public static class UserStatsMappersExtensions
{
    /// <summary>
    /// Register user stats mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterUserStatsMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<UserStatsCreateDto, UserStatsEntity>, DbUserStatsCreateMapper>();
        services.AddSingleton<IDbEntityMapper<UserStatsUpdateDto, UserStatsEntity>, DbUserStatsUpdateMapper>();
        services.AddSingleton<IDbEntityMapper<UserStatsEntity, UserStatsDto>, DbUserStatsMapper>();
        services.AddSingleton<IDbEntityMapper<RankConfigEntity, RankConfigDto>, DbRankConfigMapper>();
    }
}
