using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Bonuses.Handlers;
using Mint.App.Services.UserInteractive.Bonuses.Rules;
using Mint.App.Services.UserInteractive.Duels.Handlers;

namespace Mint.App.Services.Infrastructure.DI.UserInteractive.Bonuses;

/// <summary>
/// Register bonus services.
/// </summary>
public static class DuelsExtensions
{
    /// <summary>
    /// Register duels services.
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterDuelsHandlers(this IServiceCollection services)
    {
        services.AddScoped<IDuelHandler, DuelHandler>();
        services.AddScoped<IDuelHistoryHandler, DuelHistoryHandler>();
    }
}
