using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.UserInteractive.Bonuses.Handlers;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.Fixtures.Seeding;

namespace Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;

/// <summary>
/// Fixture for <see cref="ClaimBonusCommandHandler"/> tests with EF Core and DI.
/// </summary>
public sealed class ClaimBonusCommandHandlerFixtures : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimBonusCommandHandlerFixtures"/> class.
    /// </summary>
    public ClaimBonusCommandHandlerFixtures()
    {
        var databaseName = "TestDatabaseClaimBonus" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.RegisterAppServices();
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(databaseName));

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ProfileCommandHandler>();
        services.AddScoped<ClaimBonusCommandHandler>();

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
        ClaimBonusCommandHandlerSeeder.Seed(context);
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
        context.UserBonusStats.RemoveRange(context.UserBonusStats);
        context.Transactions.RemoveRange(context.Transactions);
        context.UserSessions.RemoveRange(context.UserSessions);
        context.Users.RemoveRange(context.Users);
        context.Accounts.RemoveRange(context.Accounts);
        context.BonusTypes.RemoveRange(context.BonusTypes);
        context.Scenarios.RemoveRange(context.Scenarios);
        context.Steps.RemoveRange(context.Steps);
        context.StepTypes.RemoveRange(context.StepTypes);
        context.Buttons.RemoveRange(context.Buttons);
        context.SystemSettings.RemoveRange(context.SystemSettings);
        await context.SaveChangesAsync(cancellationToken);

        ClaimBonusCommandHandlerSeeder.Seed(context);
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
