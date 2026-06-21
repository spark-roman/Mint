using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database;
using Mint.Database.Infrastructure;
using Mint.Database.Seeding;

namespace Mint.UnitTests.Database.Fixtures.EntityFramework;

/// <summary>
/// Repository test fixture with in-memory database
/// </summary>
public class RepositoryFixture
{
    /// <summary>
    /// Service provider
    /// </summary>
    public ServiceProvider ServiceProvider { get; init; }

    /// <summary>
    /// Database name for in-memory database
    /// </summary>
    public string DatabaseName { get; }

    /// <summary>
    /// Initial constructor
    /// </summary>
    public RepositoryFixture()
    {
        DatabaseName = "TestDatabase" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(DatabaseName));

        ServiceProvider = services.BuildServiceProvider();

        SeedDatabase();
    }

    /// <summary>
    /// Seed database with test data
    /// </summary>
    private void SeedDatabase()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        UsersSeeder.Seed(context);
        context.SaveChangesAsync().GetAwaiter().GetResult();
    }
}
