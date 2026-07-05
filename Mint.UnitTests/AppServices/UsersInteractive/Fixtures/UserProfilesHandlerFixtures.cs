using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Bonuses.Rules;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Database;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.UsersInteractive.Seeding;
using Moq;

namespace Mint.UnitTests.AppServices.UsersInteractive.Fixtures;

/// <summary>
/// Fixture for <see cref="UserProfilesHandler"/> tests with EF Core and DI.
/// </summary>
public sealed class UserProfilesHandlerFixture : IDisposable
{
    private readonly Mock<IBonusValidator> _bonusValidatorMock;
    private readonly ServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfilesHandlerFixture"/> class.
    /// </summary>
    public UserProfilesHandlerFixture()
    {
        _bonusValidatorMock = new Mock<IBonusValidator>();
        _bonusValidatorMock.Setup(v => v.CanApplyStartBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _bonusValidatorMock.Setup(v => v.CanApplyDailyBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _bonusValidatorMock.Setup(v => v.CanApplyStreakBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var databaseName = "TestDatabase" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(databaseName));

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IBonusValidator>(_ => _bonusValidatorMock.Object);
        services.AddScoped<IUserProfilesHandler, UserProfilesHandler>();

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
        UserProfilesHandlerSeeder.Seed(context);
        context.SaveChangesAsync().GetAwaiter().GetResult();
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
