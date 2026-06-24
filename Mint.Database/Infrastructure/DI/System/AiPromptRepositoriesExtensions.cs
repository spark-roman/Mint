using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.System.Repositories;

namespace Mint.Database.Infrastructure.DI.System;

/// <summary>
/// Extension methods for AI prompt repositories
/// </summary>
public static class AiPromptRepositoriesExtensions
{
    /// <summary>
    /// Register AI prompt repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterAiPromptRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAiPromptRepository, AiPromptRepository>();
    }
}
