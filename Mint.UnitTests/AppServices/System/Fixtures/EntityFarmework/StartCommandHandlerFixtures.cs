using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.Fixtures.Seeding;
using Telegram.Bot.Types;
using Moq;
using Mint.App.Services.Infrastructure.DI.System.Bot;

namespace Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;

/// <summary>
/// Fixture для тестов <see cref="StartCommandHandler"/> с EF Core и реальными репозиториями.
/// </summary>
public sealed class StartCommandHandlerFixture : IDisposable
{
    private readonly Mock<IUserProfilesHandler> _profileHandlerMock;
    private readonly ServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StartCommandHandlerFixture"/> class.
    /// </summary>
    public StartCommandHandlerFixture()
    {
        _profileHandlerMock = new Mock<IUserProfilesHandler>();

        var databaseName = "TestDatabase" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.AddStartCommandMappers();
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(databaseName));

        services.AddScoped(_ => _profileHandlerMock.Object);
        services.AddScoped<ICommandHandler, StartCommandHandler>();

        _serviceProvider = services.BuildServiceProvider();

        SeedDatabase();
    }

    /// <summary>
    /// Seeds the database with test data.
    /// </summary>
    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        StartCommandSeeder.Seed(context);
        context.SaveChangesAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets the profile handler mock.
    /// </summary>
    public Mock<IUserProfilesHandler> ProfileHandlerMock => _profileHandlerMock;

    /// <summary>
    /// Creates a service scope for resolving services.
    /// </summary>
    /// <returns>Service scope.</returns>
    public IServiceScope CreateScope()
    {
        return _serviceProvider.CreateScope();
    }

    /// <summary>
    /// Creates a mock Telegram user for testing.
    /// </summary>
    /// <param name="userId">Telegram user ID.</param>
    /// <param name="firstName">First name.</param>
    /// <param name="lastName">Last name.</param>
    /// <param name="userName">Username.</param>
    /// <returns>Mock Telegram User.</returns>
    public static User CreateMockUser(long userId = 12345, string? firstName = "Test", string? lastName = "User", string? userName = "testuser")
    {
        return new User
        {
            Id = userId,
            IsBot = false,
            FirstName = firstName ?? string.Empty,
            LastName = lastName ?? string.Empty,
            Username = userName ?? string.Empty
        };
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed) return;

        _serviceProvider?.Dispose();
        _disposed = true;
    }
}

