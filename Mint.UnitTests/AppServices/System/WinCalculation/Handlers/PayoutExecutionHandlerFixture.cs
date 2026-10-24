using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.WinCalculation.Seeding;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Handlers;

/// <summary>
/// Fixture for payout execution handler tests with EF Core and DI.
/// </summary>
public sealed class PayoutExecutionHandlerFixture : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly string _databaseName;
    private bool _disposed;

    public PayoutExecutionHandlerFixture()
    {
        _databaseName = "TestDatabasePayoutExecution" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.RegisterAppServices(new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build());
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(_databaseName));

        _serviceProvider = services.BuildServiceProvider();

        SeedDatabase();
    }

    public IServiceProvider ServiceProvider => _serviceProvider;

    public IPayoutExecutionHandler GetHandler(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<IPayoutExecutionHandler>();
    }

    public IDuelCalculationHandler GetCalculationHandler(IServiceScope scope)
    {
        return scope.ServiceProvider.GetRequiredService<IDuelCalculationHandler>();
    }

    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        DuelSettlementHandlerSeeder.Seed(context);
        context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync(cancellationToken).GetAwaiter().GetResult();
        context.Payouts.RemoveRange(context.Payouts);
        context.Transactions.RemoveRange(context.Transactions);
        context.Votes.RemoveRange(context.Votes);
        context.DuelOptions.RemoveRange(context.DuelOptions);
        context.Duels.RemoveRange(context.Duels);
        context.Accounts.RemoveRange(context.Accounts);
        context.UserStats.RemoveRange(context.UserStats);
        context.Users.RemoveRange(context.Users);
        context.UserCategories.RemoveRange(context.UserCategories);
        context.SystemSettings.RemoveRange(context.SystemSettings);

        await context.SaveChangesAsync(cancellationToken);

        DuelSettlementHandlerSeeder.Seed(context);
        await context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed) return;

        _serviceProvider?.Dispose();
        _disposed = true;
    }
}
