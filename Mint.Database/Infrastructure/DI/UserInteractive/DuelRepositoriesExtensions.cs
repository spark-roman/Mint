using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for duel repositories
/// </summary>
public static class DuelRepositoriesExtensions
{
    /// <summary>
    /// Register duel repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterDuelRepositories(this IServiceCollection services)
    {
        services.AddScoped<IDuelRepository, DuelRepository>();
    }
}
