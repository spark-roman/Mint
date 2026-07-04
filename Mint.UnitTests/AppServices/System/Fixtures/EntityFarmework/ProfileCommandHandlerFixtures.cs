using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.System.Bot.Handlers.Commands.Mappers;
using Mint.App.Services.UserInteractive.Bonuses.Rules;
using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.Users;
using Mint.Database;
using Mint.Database.Infrastructure.DI;
using Mint.Database.Infrastructure.DI.Bot;
using Mint.Database.Infrastructure.DI.Accounts;
using Mint.Database.Infrastructure.DI.System;
using Mint.Database.Infrastructure.DI.UserInteractive;
using Mint.Database.Infrastructure.DI.Users;
using Mint.UnitTests.AppServices.System.Fixtures.Seeding;
using Telegram.Bot.Types;
using Moq;

namespace Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;

/// <summary>
/// Fixture for <see cref="ProfileCommandHandler"/> tests with EF Core and DI.
/// </summary>
public sealed class ProfileCommandHandlerFixture : IDisposable
{
    private readonly Mock<IMessageFormatter> _messageFormatterMock;
    private readonly Mock<IBonusValidator> _bonusValidatorMock;
    private readonly ServiceProvider _serviceProvider;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProfileCommandHandlerFixture"/> class.
    /// </summary>
    public ProfileCommandHandlerFixture()
    {
        _messageFormatterMock = new Mock<IMessageFormatter>();
        _messageFormatterMock.Setup(f => f.FormatAsync(It.IsAny<string>(), It.IsAny<UserProfileDto>(), It.IsAny<CancellationToken>()))
            .Returns<string, UserProfileDto, CancellationToken>((template, profile, ct) => Task.FromResult($"Ваш игровой профиль"));
        _bonusValidatorMock = new Mock<IBonusValidator>();
        _bonusValidatorMock.Setup(v => v.CanApplyStartBonus(It.IsAny<UserBonusStatsDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _bonusValidatorMock.Setup(v => v.CanApplyDailyBonus(It.IsAny<UserBonusStatsDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _bonusValidatorMock.Setup(v => v.CanApplyStreakBonus(It.IsAny<UserBonusStatsDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var databaseName = "TestDatabase" + Guid.NewGuid();

        var services = new ServiceCollection();

        services.RegisterDatabaseServices();
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddDbContextFactory<MintDbContext>(options => options.UseInMemoryDatabase(databaseName));

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IDtoMapper<User, ExternalUserDto>, TgUserMapper>();
        services.AddScoped<IMessageFormatter>(_ => _messageFormatterMock.Object);
        services.AddScoped<IBonusValidator>(_ => _bonusValidatorMock.Object);
        services.AddScoped<IUserProfilesHandler, UserProfilesHandler>();
        services.AddScoped<ICommandHandler, ProfileCommandHandler>();

        _serviceProvider = services.BuildServiceProvider();

        SeedDatabase();
    }

    /// <summary>
    /// Gets the message formatter mock.
    /// </summary>
    public Mock<IMessageFormatter> MessageFormatterMock => _messageFormatterMock;

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
        ProfileCommandSeeder.Seed(context);
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
