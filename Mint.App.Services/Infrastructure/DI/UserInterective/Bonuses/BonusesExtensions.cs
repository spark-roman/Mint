using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Bonuses.Rules;

namespace Mint.App.Services.Infrastructure.DI.UserInterective.Bonuses;

/// <summary>
/// Register bonus services.
/// </summary>
public static class BonusesExtensions
{
    /// <summary>
    /// Register bonus validators.
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterBonusValidators(this IServiceCollection services)
    {
        services.AddSingleton<IBonusValidator, BonusValidator>();
    }
}
