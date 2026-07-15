using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Leaderboards;

namespace Mint.App.Services.Infrastructure.DI.UserInterective.Leaderboards;

/// <summary>
/// Extension methods for leaderboard services.
/// </summary>
public static class LeaderBoardsExtensions
{
    /// <summary>
    /// Register bonus validators.
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterLeaderboardHandlers(this IServiceCollection services)
    {
        services.AddScoped<ILeaderboardHandler, LeaderboardHandler>();
    }
}
