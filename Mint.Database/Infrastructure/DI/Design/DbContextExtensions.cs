using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.Database.Infrastructure.DI.Design;

/// <summary>
/// DbContext extension methods
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Adds the design time db context
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void AddMintDesignTimeDbContext(this IServiceCollection services)
    {
        services.AddScoped<IDesignTimeDbContextFactory<MintDbContext>, MintDesignTimeDbContextFactory>();
    }
}
