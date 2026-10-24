using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI.System;
using Mint.App.Services.Infrastructure.DI.System.Bot;
using Mint.App.Services.Infrastructure.DI.System.Duels;
using Mint.App.Services.Infrastructure.DI.System.News;
using Mint.App.Services.Infrastructure.DI.System.Referral;
using Mint.App.Services.Infrastructure.DI.System.Settings;
using Mint.App.Services.Infrastructure.DI.UserInteractive.Bonuses;
using Mint.App.Services.Infrastructure.DI.UserInteractive.Leaderboards;
using Mint.App.Services.Infrastructure.DI.UserInteractive.Profiles;
using Mint.App.Services.System.DuelsGeneration.Dto;

namespace Mint.App.Services.Infrastructure.DI;

/// <summary>
/// DI extension methods for appservices
/// </summary>
public static class RegisterAppServicesExtensions
{
    /// <summary>
    /// Registers all app services.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configuration">Application configuration.</param>
    public static void RegisterAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        services.Configure<DeepSeekSettings>(configuration.GetSection(DeepSeekSettings.SectionName));
        services.Configure<ReferralSettings>(configuration.GetSection(ReferralSettings.SectionName));

        services.RegisterDuelGenerationServices();
        services.RegisterDuelMappers();

        services.RegisterBotServices();

        services.RegisterUserProfileHandlers();
        services.RegisterBonusValidators();
        services.RegisterLeaderboardHandlers();
        services.RegisterDuelsHandlers();
        services.RegisterNewsServices();
        services.RegisterDuelsServices();
        services.RegisterSettingsServices();
        services.RegisterReferralServices();
    }
}
