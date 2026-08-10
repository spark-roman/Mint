using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.WinCalculationRules;

namespace Mint.App.Services.Infrastructure.DI.System.Duels;

/// <summary>
/// DI extension methods for rules
/// </summary>
public static class RulesExtensions
{
    /// <summary>
    /// Register rules for calculation services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterCalculationRulesServices(this IServiceCollection services)
    {
        services.AddScoped<OpinionMatchRule>();
        services.AddScoped(sp => new ReadOnlyCollection<IWinCalculationRule>(
        [
            sp.GetRequiredService<OpinionMatchRule>()
        ]));
    }
}
