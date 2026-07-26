using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.App.Services.System.WinCalculation.Jobs;
using Mint.App.Services.System.WinCalculation.Services;

namespace Mint.App.Services.Infrastructure.DI.System.Duels;

/// <summary>
/// DI extension methods for Duels
/// </summary>
public static class DuelsExtensions
{
    /// <summary>
    /// Register duel services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterDuelsServices(this IServiceCollection services)
    {
        services.AddScoped<IDuelCalculationHandler, DuelCalculationHandler>();
        services.AddScoped<IDuelSettlementHandler, DuelSettlementHandler>();
        services.AddScoped<IDuelPayoutService, DuelPayoutService>();
        services.AddScoped<DuelPayoutJob>();
    }
}
