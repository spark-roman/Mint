using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Mappers;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for bot mappers
/// </summary>
public static class BotMappersExtensions
{
    /// <summary>
    /// Registers all bot entity mappers.
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterBotMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<ScenarioEntity, ScenarioDto>, DbScenarioMapper>();
        services.AddSingleton<IDbEntityMapper<StepEntity, StepDto>, DbStepMapper>();
        services.AddSingleton<IDbEntityMapper<ButtonEntity, ButtonDto>, DbButtonMapper>();
    }
}
