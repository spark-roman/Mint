using Mint.Common.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Mappers;
using Mint.Database.Entities.UserInteractive.Duels;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for duel mappers
/// </summary>
public static class DuelMappersExtensions
{
    /// <summary>
    /// Register duel mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterDuelMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<DuelCreateDto, DuelEntity>, DbDuelCreateMapper>();
        services.AddSingleton<IDbEntityMapper<DuelEntity, DuelDto>, DbDuelMapper>();
    }
}
