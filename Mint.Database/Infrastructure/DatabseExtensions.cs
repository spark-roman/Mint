using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Infrastructure.Accounts;
using Mint.Database.Infrastructure.Users;

namespace Mint.Database.Infrastructure;

/// <summary>
/// Database extensions
/// </summary>
public static class DatabseExtensions
{
    /// <summary>
    /// Register entity mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterDatabaseServices(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        
        services.RegisterUserMappers();
        services.RegisterUserRepositories();
        services.RegisterAccountMappers();
        services.RegisterAccountRepositories();
    }
}
