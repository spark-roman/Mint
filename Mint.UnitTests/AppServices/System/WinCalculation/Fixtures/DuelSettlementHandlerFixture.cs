using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.WinCalculation.Seeding;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Fixtures;

/// <summary>
/// Fixture for duel settlement handler tests with EF Core and DI.
/// </summary>
public sealed class DuelSettlementHandlerFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly string _databaseName;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelSettlementHandlerFixture"/> class.
    /// </summary>
    public DuelSettlementHandlerFixture()
    {
        _databaseName = "TestDatabaseDuelSettlement" + Guid.NewGuid();

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
    /// Gets the duel settlement handler from DI.
    /// </summary>
    /// <param name="scope">Service scope.</param>
    /// <returns>The duel settlement handler instance.</returns>
    public IDuelSettlementHandler GetHandler(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<IDuelSettlementHandler>();
    }

    /// <summary>
    /// Gets the duel calculation handler from DI.
    /// </summary>
    /// <param name="scope">Service scope.</param>
    /// <returns>The duel calculation handler instance.</returns>
    public IDuelCalculationHandler GetCalculationHandler(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<IDuelCalculationHandler>();
    }

    /// <summary>
    /// Seeds the database with test data.
    /// </summary>
    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        DuelSettlementHandlerSeeder.Seed(context);
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
        context.Accounts.RemoveRange(context.Accounts);
        context.UserStats.RemoveRange(context.UserStats);
        context.Users.RemoveRange(context.Users);
        context.UserCategories.RemoveRange(context.UserCategories);

        await context.SaveChangesAsync(cancellationToken);

        DuelSettlementHandlerSeeder.Seed(context);
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
