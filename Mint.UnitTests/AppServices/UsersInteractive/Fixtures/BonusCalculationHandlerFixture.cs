using AdvApplication.Auth.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Bonuses.Handlers;
using Mint.App.Services.UserInteractive.Bonuses.Rules;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Database;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.UsersInteractive.Seeding;
using Moq;

namespace Mint.UnitTests.AppServices.UsersInteractive.Fixtures;

/// <summary>
/// Fixture for <see cref="BonusCalculationHandler"/> tests with EF Core and DI.
/// </summary>
public sealed class BonusCalculationHandlerFixture : IDisposable
{
    private readonly Mock<IBonusValidator> _bonusValidatorMock;
    private readonly ServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BonusCalculationHandlerFixture"/> class.
    /// </summary>
    public BonusCalculationHandlerFixture()
    {
        _bonusValidatorMock = new Mock<IBonusValidator>();
        _bonusValidatorMock.Setup(v => v.CanApplyStartBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _bonusValidatorMock.Setup(v => v.CanApplyDailyBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _bonusValidatorMock.Setup(v => v.CanApplyStreakBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var databaseName = "TestDatabaseBonusCalc" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(databaseName));

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IBonusValidator>(_ => _bonusValidatorMock.Object);
        services.AddScoped<IBonusCalculationHandler, BonusCalculationHandler>();

        _serviceProvider = services.BuildServiceProvider();

        SeedDatabase();
    }

    /// <summary>
    /// Gets the bonus validator mock.
    /// </summary>
    public Mock<IBonusValidator> BonusValidatorMock => _bonusValidatorMock;

    /// <summary>
    /// Seeds the database with test data.
    /// </summary>
    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        BonusCalculationHandlerSeeder.Seed(context);
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
        context.Users.RemoveRange(context.Users);
        context.Accounts.RemoveRange(context.Accounts);
        context.BonusTypes.RemoveRange(context.BonusTypes);
        await context.SaveChangesAsync(cancellationToken);

        BonusCalculationHandlerSeeder.Seed(context);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Creates a service scope for resolving services.
    /// </summary>
    /// <returns>Service scope.</returns>
    public IServiceScope CreateScope()
    {
        return _serviceProvider.CreateScope();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;

        _serviceProvider?.Dispose();
        _disposed = true;
    }
}
