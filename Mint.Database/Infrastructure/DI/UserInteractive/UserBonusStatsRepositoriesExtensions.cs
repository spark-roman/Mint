using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for user bonus stats repositories
/// </summary>
public static class UserBonusStatsRepositoriesExtensions
{
    /// <summary>
    /// Register user bonus stats repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterUserBonusStatsRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserBonusStatsRepository, UserBonusStatsRepository>();
    }
}
