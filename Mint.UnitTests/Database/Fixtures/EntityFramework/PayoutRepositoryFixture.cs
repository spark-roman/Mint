using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Users;
using Mint.Database.Seeding;

namespace Mint.UnitTests.Database.Fixtures.EntityFramework;

/// <summary>
/// Repository test fixture for payout tests.
/// Seeds system user (Id=1) with system account (Id=1) and huge balance,
/// plus regular test users (2, 3) with their accounts.
/// </summary>
public class PayoutRepositoryFixture
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
    public PayoutRepositoryFixture()
    {
        DatabaseName = "PayoutTestDatabase" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(DatabaseName));

        ServiceProvider = services.BuildServiceProvider();

        SeedDatabase();
    }

    /// <summary>
    /// Seed database with system user/account and regular test users.
    /// </summary>
    private void SeedDatabase()
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        
        PayoutUsersSeeder.Seed(context);

        context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Clears all payout test data from the database and re-seeds.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        using var scope = ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
        context.Payouts.RemoveRange(context.Payouts);
        context.Votes.RemoveRange(context.Votes);
        context.Transactions.RemoveRange(context.Transactions);
        context.Duels.RemoveRange(context.Duels);
        context.DuelOptions.RemoveRange(context.DuelOptions);
        context.Users.RemoveRange(context.Users);
        context.Accounts.RemoveRange(context.Accounts);

        await context.SaveChangesAsync(cancellationToken);

        SeedDatabase();

        await context.SaveChangesAsync(cancellationToken);
    }
}
