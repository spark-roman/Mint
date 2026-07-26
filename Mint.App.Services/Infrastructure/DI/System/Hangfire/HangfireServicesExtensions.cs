using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;

namespace Mint.App.Services.Infrastructure.DI.System.Hangfire;

/// <summary>
/// DI container extension for working with hangfire
/// </summary> 
public static class HangfireServicesExtensions
{
    /// <summary>
    /// Add hanfire services
    /// </summary>
    /// <param name="builder">App builder</param>
    /// <param name="connectionString">Connection string for db</param>
    public static void AddHangfireServices(this WebApplicationBuilder builder, string connectionString)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString)));

        builder.Services.AddHangfireServer(options =>
        {
            options.WorkerCount = 1;
        });
    }
}
