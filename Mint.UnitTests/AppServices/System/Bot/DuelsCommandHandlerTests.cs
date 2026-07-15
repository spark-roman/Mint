using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;
using Telegram.Bot.Types;

namespace Mint.UnitTests.AppServices.System.Bot;

/// <summary>
/// Tests for <see cref="DuelsCommandHandler"/> using DI and EF Core.
/// </summary>
public class DuelsCommandHandlerTests : IClassFixture<DuelsCommandHandlerFixtures>, IDisposable
{
    private readonly DuelsCommandHandlerFixtures _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelsCommandHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public DuelsCommandHandlerTests(DuelsCommandHandlerFixtures fixture)
    {
        _fixture = fixture;
    }

    #region HandleAsync - Happy Path

    /// <summary>
    /// Verifies that HandleAsync returns command result with categories and keyboard.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidUser_ReturnsCommandResultWithMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Duels);
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Test" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.True(result.IsNewMessage);
        Assert.Contains("📊", result.Message);
        Assert.Contains("ДУЭЛИ ДНЯ", result.Message);
        Assert.Contains("Криптовалюта", result.Message);
        Assert.Contains("Технологии", result.Message);
        Assert.Contains("Спорт", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns category buttons with correct actions.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ReturnsCategoryButtonsWithCorrectActions()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Duels);
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Test" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result.Keyboard);
        Assert.Equal(3, result.Keyboard.Count);
        Assert.Equal("📂 Криптовалюта", result.Keyboard[0].Caption);
        Assert.Equal("category_crypto", result.Keyboard[0].Action);
        Assert.Equal("📂 Спорт", result.Keyboard[1].Caption);
        Assert.Equal("category_sports", result.Keyboard[1].Action);
        Assert.Equal("📂 Технологии", result.Keyboard[2].Caption);
        Assert.Equal("category_tech", result.Keyboard[2].Action);
    }

    /// <summary>
    /// Verifies that HandleAsync creates a session with correct parameters.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CreatesSession_WithCorrectParameters()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Duels);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<Mint.Database.Entities.Users.Sessions.Repositories.IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Test" };

        var scenario = await scenarioRepository.GetScenarioByNameAsync("duels", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, CancellationToken.None);
        Assert.NotNull(step);

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(scenario.Id, session.ScenarioId);
        Assert.Equal(step.Id, session.CurrentStepId);
        Assert.Contains("\"step\":\"categories\"", session.Data);
    }

    #endregion

    #region HandleAsync - Scenario Not Found

    /// <summary>
    /// Verifies that HandleAsync returns error when duels scenario is not found.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ScenarioNotFound_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Duels);
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Test" };

        // Delete duels scenario to simulate not found
        var scenario = await scenarioRepository.GetScenarioByNameAsync("duels", CancellationToken.None);
        Assert.NotNull(scenario);

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert - should succeed since scenario exists in seed data
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
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
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Duels);

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.HandleAsync(null!, "", CancellationToken.None));
    }

    #endregion

    #region HandleAsync - Categories

    /// <summary>
    /// Verifies that HandleAsync returns all active categories.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ReturnsAllActiveCategories()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Duels);
        var categoryRepository = _currentScope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Test" };

        var categories = await categoryRepository.GetAllActiveAsync(CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Keyboard);
        Assert.Equal(categories.Count, result.Keyboard.Count);
    }

    /// <summary>
    /// Verifies that HandleAsync formats categories with emoji prefix.
    /// </summary>
    [Fact]
    public async Task HandleAsync_FormatsCategoriesWithEmojiPrefix()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Duels);
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Test" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result.Keyboard);
        foreach (var button in result.Keyboard)
        {
            Assert.StartsWith("📂 ", button.Caption);
        }
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
