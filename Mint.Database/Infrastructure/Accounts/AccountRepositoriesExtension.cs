using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.Accounts;
using Mint.Database.Entities.Accounts.Repositories;

namespace Mint.Database.Infrastructure.Accounts;

/// <summary>
/// Account repositories extension.
/// </summary>
public static class AccountRepositoriesExtension
{
    /// <summary>
    /// Register account repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterAccountRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountRepository, AccountRepository>();
    }
}
