using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.Infrastructure.DI;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.Fixtures.Seeding;

namespace Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;

/// <summary>
/// Fixture for button click handler tests with EF Core and DI.
/// </summary>
public sealed class ButtonClickHandlerFixtures : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly string _databaseName;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonClickHandlerFixtures"/> class.
    /// </summary>
    public ButtonClickHandlerFixtures()
    {
        _databaseName = "TestDatabaseButtonClick" + Guid.NewGuid();

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
        ButtonClickHandlerSeeder.Seed(context);
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
        context.Duels.RemoveRange(context.Duels);
        context.DuelOptions.RemoveRange(context.DuelOptions);
        context.UserCategories.RemoveRange(context.UserCategories);
        context.UserSessions.RemoveRange(context.UserSessions);
        context.Users.RemoveRange(context.Users);
        context.Accounts.RemoveRange(context.Accounts);
        context.Steps.RemoveRange(context.Steps);
        context.Scenarios.RemoveRange(context.Scenarios);
        context.StepTypes.RemoveRange(context.StepTypes);
        context.Buttons.RemoveRange(context.Buttons);
        await context.SaveChangesAsync(cancellationToken);

        ButtonClickHandlerSeeder.Seed(context);
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
