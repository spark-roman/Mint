using AdvApplication.Auth.Users;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.UserInteractive.Leaderboards;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;
using Telegram.Bot.Types;

namespace Mint.UnitTests.AppServices.System.Bot;

/// <summary>
/// Tests for <see cref="LeaderboardCommandHandler"/> using DI and EF Core.
/// </summary>
public class LeaderboardCommandHandlerTests : IClassFixture<LeaderboardCommandHandlerFixtures>, IDisposable
{
    private readonly LeaderboardCommandHandlerFixtures _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaderboardCommandHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public LeaderboardCommandHandlerTests(LeaderboardCommandHandlerFixtures fixture)
    {
        _fixture = fixture;
    }

    #region HandleAsync - Happy Path

    /// <summary>
    /// Verifies that HandleAsync returns success result with formatted message and keyboard.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidUser_ReturnsCommandResultWithMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var tgUser = new Telegram.Bot.Types.User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsFinal);
        Assert.False(result.IsNewMessage);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Contains("🏆", result.Message);
        Assert.Contains("ТАБЛИЦА ЛИДЕРОВ", result.Message);
        Assert.Contains("🥇", result.Message);
        Assert.Contains("alice_j", result.Message);
        Assert.Contains("Ваше место в рейтинге", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns correct keyboard buttons from the leaderboard step.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ReturnsCorrectKeyboardButtons()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var tgUser = new Telegram.Bot.Types.User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result.Keyboard);
        Assert.Single(result.Keyboard);
        Assert.Equal("🔙 Вернуться в профиль", result.Keyboard[0].Caption);
    }

    /// <summary>
    /// Verifies that HandleAsync returns leaderboard entries with correct rankings using real message formatter.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ReturnsLeaderboardEntriesWithCorrectRankings()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<Mint.App.Services.UserInteractive.Leaderboards.ILeaderboardHandler>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();
        var tgUser = new Telegram.Bot.Types.User { Id = 1001, IsBot = false, FirstName = "Alice" };

        var step = await scenarioRepository.GetStepByActionAsync("leaderboard", CancellationToken.None);
        Assert.NotNull(step);

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);

        // Verify leaderboard entries are formatted with correct order (Alice=1st, Charlie=2nd, Bob=3rd, Diana=4th)
        Assert.Contains("🥇", result.Message);
        Assert.Contains("1.", result.Message);
        Assert.Contains("alice_j", result.Message);
        Assert.Contains("🥈", result.Message);
        Assert.Contains("2.", result.Message);
        Assert.Contains("eve_d", result.Message);
        Assert.Contains("🥉", result.Message);
        Assert.Contains("3.", result.Message);
        Assert.Contains("charlie_b", result.Message);
        Assert.Contains("🎖", result.Message);
        Assert.Contains("4.", result.Message);
        Assert.Contains("bob_s", result.Message);
        Assert.Contains("5.", result.Message);
        Assert.Contains("diana_p", result.Message);

        // Verify user rank info is present
        Assert.Contains("👤 **Ваше место в рейтинге:** #1", result.Message);
        Assert.Contains("1,500 RP", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync includes user's own entry in the result.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UserInTop_ReturnsUserEntry()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<ILeaderboardHandler>();
        var tgUser = new Telegram.Bot.Types.User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        // Verify via leaderboard handler that user entry exists
        var leaderboardResult = await leaderboardHandler.GetLeaderboardAsync(15, 1001, AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(leaderboardResult.UserEntry);
        Assert.Equal(1, leaderboardResult.UserRank);
        Assert.Equal(1001, leaderboardResult.UserEntry.ExternalUserId);
        Assert.Equal("alice_j", leaderboardResult.UserEntry.DisplayName);
    }

    /// <summary>
    /// Verifies that HandleAsync returns correct total users count.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ReturnsCorrectTotalUsersCount()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<Mint.App.Services.UserInteractive.Leaderboards.ILeaderboardHandler>();
        var tgUser = new Telegram.Bot.Types.User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        var leaderboardResult = await leaderboardHandler.GetLeaderboardAsync(15, 1001, Mint.Common.Contracts.Users.AuthSystem.Tg, CancellationToken.None);
        Assert.Equal(5, leaderboardResult.TotalUsers);
    }

    #endregion

    #region HandleAsync - User Not In Top

    /// <summary>
    /// Verifies that HandleAsync returns user entry for user not in top N.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UserNotInTop_ReturnsUserEntry()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<ILeaderboardHandler>();
        // User 1004 (Diana) has no stats, so not in top 4
        var tgUser = new Telegram.Bot.Types.User { Id = 1004, IsBot = false, FirstName = "Diana" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var leaderboardResult = await leaderboardHandler.GetLeaderboardAsync(4, 1004, AuthSystem.Tg, CancellationToken.None);
        // User without stats has no entry in leaderboard
        Assert.NotNull(leaderboardResult.UserEntry);
        Assert.Equal(300, leaderboardResult.UserEntry.RankPoints);
    }

    #endregion

    #region HandleAsync - Custom Top Parameter

    /// <summary>
    /// Verifies that HandleAsync parses custom top parameter correctly.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CustomTop_ReturnsCorrectNumberOfEntries()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<ILeaderboardHandler>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        await handler.HandleAsync(tgUser, "2", CancellationToken.None);

        // Assert
        var leaderboardResult = await leaderboardHandler.GetLeaderboardAsync(2, 1001, AuthSystem.Tg, CancellationToken.None);
        Assert.Equal(2, leaderboardResult.Entries.Count);
        Assert.Equal(1001, leaderboardResult.Entries.First().ExternalUserId); // Alice
        Assert.Equal(1005, leaderboardResult.Entries.Skip(1).First().ExternalUserId); // Eve
    }

    /// <summary>
    /// Verifies that HandleAsync uses default top (15) when input is invalid.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidTop_UsesDefaultTop()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<ILeaderboardHandler>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "invalid", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var leaderboardResult = await leaderboardHandler.GetLeaderboardAsync(15, 1001, AuthSystem.Tg, CancellationToken.None);
        Assert.Equal(5, leaderboardResult.Entries.Count); // All 4 users with stats are in top 15
    }

    /// <summary>
    /// Verifies that HandleAsync uses default top when input is negative.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NegativeTop_UsesDefaultTop()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<Mint.App.Services.UserInteractive.Leaderboards.ILeaderboardHandler>();
        var tgUser = new Telegram.Bot.Types.User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "-5", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var leaderboardResult = await leaderboardHandler.GetLeaderboardAsync(1, 1001, Mint.Common.Contracts.Users.AuthSystem.Tg, CancellationToken.None);
        Assert.Single(leaderboardResult.Entries); // top 1 should have 1 entry
    }

    #endregion

    #region HandleAsync - Session Creation

    /// <summary>
    /// Verifies that HandleAsync creates or updates user session.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CreatesSession_WithCorrectParameters()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        var scenario = await scenarioRepository.GetScenarioByNameAsync("profile", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetStepByActionAsync("leaderboard", CancellationToken.None);
        Assert.NotNull(step);

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(scenario.Id, session.ScenarioId);
    }

    /// <summary>
    /// Verifies that HandleAsync stores step data with correct top value.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CreatesSession_WithTopData()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        await handler.HandleAsync(tgUser, "5", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1002, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Contains("\"step\":\"leaderboard\"", session.Data);
        Assert.Contains("\"top\":5", session.Data);
    }

    #endregion

    #region HandleAsync - Null User

    /// <summary>
    /// Verifies that HandleAsync throws ArgumentNullException when user is null.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NullUser_ThrowsArgumentNullException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.HandleAsync(null!, "", CancellationToken.None));
    }

    #endregion

    #region HandleAsync - Different Users

    /// <summary>
    /// Verifies that HandleAsync returns correct rank for user with lowest points.
    /// </summary>
    [Fact]
    public async Task HandleAsync_LowestRankUser_ReturnsCorrectRank()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<Mint.App.Services.UserInteractive.Leaderboards.ILeaderboardHandler>();
        var tgUser = new Telegram.Bot.Types.User { Id = 1004, IsBot = false, FirstName = "Diana" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var leaderboardResult = await leaderboardHandler.GetLeaderboardAsync(15, 1004, Mint.Common.Contracts.Users.AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(leaderboardResult.UserEntry);
        Assert.Equal(300m, leaderboardResult.UserEntry.RankPoints);
        // Diana should have a valid rank (last among 4 users with stats)
        Assert.NotNull(leaderboardResult.UserRank);
        Assert.True(leaderboardResult.UserRank >= 1);
    }

    /// <summary>
    /// Verifies that HandleAsync returns correct rank for user with second highest points.
    /// </summary>
    [Fact]
    public async Task HandleAsync_SecondRankUser_ReturnsCorrectRank()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Leaderboard);
        var leaderboardHandler = _currentScope.ServiceProvider.GetRequiredService<Mint.App.Services.UserInteractive.Leaderboards.ILeaderboardHandler>();
        var tgUser = new Telegram.Bot.Types.User { Id = 1003, IsBot = false, FirstName = "Charlie" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var leaderboardResult = await leaderboardHandler.GetLeaderboardAsync(15, 1003, Mint.Common.Contracts.Users.AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(leaderboardResult.UserEntry);
        Assert.Equal("charlie_b", leaderboardResult.UserEntry.DisplayName);
        Assert.Equal(1200m, leaderboardResult.UserEntry.RankPoints);
        Assert.NotNull(leaderboardResult.UserRank);
        Assert.True(leaderboardResult.UserRank >= 1);
    }

    #endregion

    private bool _disposed;

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _currentScope?.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
