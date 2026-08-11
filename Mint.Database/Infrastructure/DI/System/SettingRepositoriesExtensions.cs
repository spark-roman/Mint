using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.System.Settings.Repositories;

namespace Mint.Database.Infrastructure.DI.System;

/// <summary>
/// Extension methods for setting repositories
/// </summary>
public static class SettingRepositoriesExtensions
{
    /// <summary>
    /// Register setting repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterSettingRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
    }
}