using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
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

    /// <summary>
    /// Clears all duel test data from the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Duels.RemoveRange(context.Duels);
        context.DuelOptions.RemoveRange(context.DuelOptions);
        context.Users.RemoveRange(context.Users);
        context.Accounts.RemoveRange(context.Accounts);
        context.UserStats.RemoveRange(context.UserStats);
        context.UserCategories.RemoveRange(context.UserCategories);
        context.UserBonusStats.RemoveRange(context.UserBonusStats);
        context.RankConfigs.RemoveRange(context.RankConfigs);
        context.StepTypes.RemoveRange(context.StepTypes);
        context.Scenarios.RemoveRange(context.Scenarios);
        context.Steps.RemoveRange(context.Steps);
        context.Buttons.RemoveRange(context.Buttons);

        await context.SaveChangesAsync(cancellationToken);

        UsersSeeder.Seed(context);

        await context.SaveChangesAsync(cancellationToken);
    }
}
