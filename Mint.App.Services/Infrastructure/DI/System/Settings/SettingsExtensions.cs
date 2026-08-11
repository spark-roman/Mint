using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Settings.Handlers;

namespace Mint.App.Services.Infrastructure.DI.System.Settings;

/// <summary>
/// DI extension methods for Settings
/// </summary>
public static class SettingsExtensions
{
    /// <summary>
    /// Register settings services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterSettingsServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<ISystemSettingHandler, SystemSettingHandler>();
    }
}
