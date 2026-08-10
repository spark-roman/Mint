using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI;
using Mint.App.Services.Infrastructure.DI.System.Duels;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.WinCalculation.Seeding;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Fixtures;

/// <summary>
/// Fixture for duel calculation handler tests with EF Core In-Memory and DI.
/// </summary>
public sealed class DuelCalculationHandlerFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly string _databaseName;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelCalculationHandlerFixture"/> class.
    /// </summary>
    public DuelCalculationHandlerFixture()
    {
        _databaseName = "TestDatabaseDuelCalculation" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.RegisterDuelsServices();
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
    /// Gets the duel calculation handler from DI.
    /// </summary>
    /// <param name="scope">Service scope.</param>
    /// <returns>The duel calculation handler instance.</returns>
    public IDuelCalculationHandler GetHandler(IServiceScope scope)
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
        DuelCalculationHandlerSeeder.Seed(context);
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
        context.Votes.RemoveRange(context.Votes);
        context.DuelOptions.RemoveRange(context.DuelOptions);
        context.Duels.RemoveRange(context.Duels);
        context.Accounts.RemoveRange(context.Accounts);
        context.Users.RemoveRange(context.Users);

        await context.SaveChangesAsync(cancellationToken);

        DuelCalculationHandlerSeeder.Seed(context);
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
