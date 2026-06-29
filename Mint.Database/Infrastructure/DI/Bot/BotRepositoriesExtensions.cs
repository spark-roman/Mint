using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.Bot.Commands.Repositories;

namespace Mint.Database.Infrastructure.DI.Bot;

/// <summary>
/// Bot repositories extensions.
/// </summary>
public static class BotRepositoriesExtensions
{
    /// <summary>
    /// Registers all bot repositories.
    /// </summary>
    /// <param name="services">Service collection.</param>
    public static void RegisterBotRepositories(this IServiceCollection services)
    {
        services.AddScoped<IScenarioRepository, ScenarioRepository>();
    }
}
