using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for user stats repositories
/// </summary>
public static class UserStatsRepositoriesExtensions
{
    /// <summary>
    /// Register user stats repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterUserStatsRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserStatsRepository, UserStatsRepository>();
        services.AddScoped<IRankConfigRepository, RankConfigRepository>();
    }
}
