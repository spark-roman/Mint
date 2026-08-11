using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.System.Payouts.Repositories;

namespace Mint.Database.Infrastructure.DI.System;

/// <summary>
/// Extension methods for payout repositories
/// </summary>
public static class PayoutRepositoriesExtensions
{
    /// <summary>
    /// Register payout repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterPayoutRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPayoutRepository, PayoutRepository>();
    }
}
