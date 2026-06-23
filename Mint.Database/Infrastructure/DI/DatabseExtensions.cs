using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Infrastructure.DI.Accounts;
using Mint.Database.Infrastructure.DI.Transactions;
using Mint.Database.Infrastructure.DI.UserInteractive;
using Mint.Database.Infrastructure.DI.Users;

namespace Mint.Database.Infrastructure.DI;

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
        services.RegisterTransactionMappers();
        services.RegisterTransactionRepositories();
        services.RegisterDuelMappers();
        services.RegisterDuelRepositories();
        services.RegisterVoteMappers();
        services.RegisterVoteRepositories();
    }
}
