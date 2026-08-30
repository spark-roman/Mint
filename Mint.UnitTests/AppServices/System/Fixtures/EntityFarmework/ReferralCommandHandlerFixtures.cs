using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mint.App.Services.Infrastructure.DI;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Referral.Dto;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.UnitTests.AppServices.System.Fixtures.Seeding;
using Moq;
using Telegram.Bot.Types;

namespace Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;

/// <summary>
/// Fixture for <see cref="ReferralCommandHandler"/> tests with EF Core and DI.
/// </summary>
public sealed class ReferralCommandHandlerFixture : IDisposable
{
    private const string BotUserName = "opinion_test_bot";
    private readonly ServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferralCommandHandlerFixture"/> class.
    /// </summary>
    public ReferralCommandHandlerFixture()
    {
        var databaseName = "TestDatabase" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(databaseName));

        services.AddSingleton(TimeProvider.System);
        services.RegisterAppServices("salt", 8);
        services.Configure<TelegramOptions>(options => options.BotUsername = BotUserName);

        _serviceProvider = services.BuildServiceProvider();

        SeedDatabase();
    }

    /// <summary>
    /// Gets the configured bot username.
    /// </summary>
    public string ConfiguredBotUserName => BotUserName;

    /// <summary>
    /// Seeds the database with test data.
    /// </summary>
    private void SeedDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();

        using var context = dbContextFactory.CreateDbContextAsync().GetAwaiter().GetResult();
        ReferralCommandSeeder.Seed(context);
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
