using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.Transactions.Repositories;

namespace Mint.Database.Infrastructure.Transactions;

/// <summary>
/// Extension methods for transaction repositories
/// </summary>
public static class TransactionRepositoriesExtensions
{
    /// <summary>
    /// Register transaction repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterTransactionRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITransactionRepository, TransactionRepository>();
    }
}
