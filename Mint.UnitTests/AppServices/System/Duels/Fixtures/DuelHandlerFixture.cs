using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.Duels.Seeding;

namespace Mint.UnitTests.AppServices.System.Duels.Fixtures;

/// <summary>
/// Fixture for duel handler tests with EF Core and DI.
/// </summary>
public sealed class DuelHandlerFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly string _databaseName;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelHandlerFixture"/> class.
    /// </summary>
    public DuelHandlerFixture()
    {
        _databaseName = "TestDatabaseDuelHandler" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.RegisterAppServices();
        
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(_databaseName));

        _serviceProvider = services.BuildServiceProvider();

        SeedDatabase();
    }

    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public IServiceProvider ServiceProvider => _serviceProvider;

    /// <summary>
    /// Seeds the database with test data.
    /// </summary>
    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        DuelHandlerSeeder.Seed(context);
        context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Resets the database to the initial seed state.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync(cancellationToken).GetAwaiter().GetResult();
        context.Transactions.RemoveRange(context.Transactions);
        context.Votes.RemoveRange(context.Votes);
        context.DuelOptions.RemoveRange(context.DuelOptions);
        context.Duels.RemoveRange(context.Duels);
        context.Users.RemoveRange(context.Users);
        context.Accounts.RemoveRange(context.Accounts);
        context.UserCategories.RemoveRange(context.UserCategories);

        await context.SaveChangesAsync(cancellationToken);

        DuelHandlerSeeder.Seed(context);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;

        _serviceProvider?.Dispose();
        _disposed = true;
    }
}
