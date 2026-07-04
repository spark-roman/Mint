using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI.System;
using Mint.App.Services.Infrastructure.DI.UserInterective.Bonuses;
using Mint.App.Services.Infrastructure.DI.UserInterective.Profiles;
using Mint.App.Services.System.DuelsGeneration;
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
    public static void RegisterAppServices(this IServiceCollection services)
    {
        services.AddSingleton(new DeepSeekSettings
        {
            DeepSeekApiUrl = new Uri("http://localhost:8080/chat"),
            Token = "test-token"
        });

        services.AddHttpClient<IDuelGenerationService, DuelGenerationService>();
        services.RegisterDuelGenerationServices();
        services.RegisterDuelMappers();

        services.AddStartCommandMappers();

        services.RegisterUserProfileHandlers();
        services.RegisterBonusValidators();
    }
}
