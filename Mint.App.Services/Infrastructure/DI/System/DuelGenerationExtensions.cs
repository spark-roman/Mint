using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.DuelsGeneration;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.Mappers;
using Mint.App.Services.System.DuelsGeneration.Prompts;
using Mint.App.Services.System.DuelsGeneration.Validators;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.Infrastructure.DI.System;

/// <summary>
/// DI extension methods for duels
/// </summary>
public static class DuelGenerationExtensions
{
    /// <summary>
    /// Register duels generation services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterDuelGenerationServices(this IServiceCollection services)
    {
        services.AddSingleton<IPromptsGenerator, PromptsGenerator>();
        services.AddSingleton<IDuelGenerationService, DuelGenerationService>();
        services.AddSingleton<IDuelGenerationValidator, DuelGenerationValidator>();
    }

    /// <summary>
    /// Register duels mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterDuelMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDtoMapper<DuelGenerationDto, DuelCreateDto>, DuelMapper>();
    }

    /// <summary>
    /// Register HttpClient for DeepSeek API calls
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="httpClient">HTTP client to register</param>
    public static void RegisterDeepSeekHttpClient(this IServiceCollection services, HttpClient httpClient)
    {
        services.AddSingleton(httpClient);
    }
}
