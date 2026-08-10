using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.DuelsGeneration.Handlers;
using Mint.App.Services.System.DuelsGeneration.Jobs;
using Mint.App.Services.System.DuelsGeneration.Services;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.App.Services.System.WinCalculation.Jobs;
using Mint.App.Services.System.WinCalculation.Services;
using Mint.App.Services.System.WinCalculation.WinCalculationRules;

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
        services.AddScoped<IDuelGenerationHandler, DuelGenerationHandler>();
        services.AddScoped<IDuelPayoutService, DuelPayoutService>();
        services.AddScoped<IDuelPublishService, DuelPublishService>();
        services.AddScoped<DuelPayoutJob>();
        services.AddScoped<DuelPublishJob>();

        services.RegisterCalculationRulesServices();
    }
}
