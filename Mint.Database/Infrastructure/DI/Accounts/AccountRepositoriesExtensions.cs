using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Accounts.Repositories;

namespace Mint.Database.Infrastructure.DI.Accounts;

/// <summary>
/// Account repositories extension.
/// </summary>
public static class AccountRepositoriesExtensionss
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
