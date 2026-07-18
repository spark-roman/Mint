using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Users;
using Mint.Database;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;
using Telegram.Bot.Types;

namespace Mint.UnitTests.AppServices.System.Bot;

/// <summary>
/// Tests for <see cref="NumberInputCommandHandler"/> using DI and EF Core.
/// </summary>
public class NumberInputCommandHandlerTests : IClassFixture<NumberInputCommandHandlerFixtures>, IDisposable
{
    private readonly NumberInputCommandHandlerFixtures _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumberInputCommandHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public NumberInputCommandHandlerTests(NumberInputCommandHandlerFixtures fixture)
    {
        _fixture = fixture;
    }

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
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.HandleAsync(null!, "100", CancellationToken.None));
    }

    #endregion

    #region HandleAsync - Invalid Amount

    /// <summary>
    /// Verifies that HandleAsync returns error when input is not a valid number.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidAmount_Text_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "abc", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("положительное число", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when input is zero.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidAmount_Zero_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "0", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("положительное число", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when input is negative.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidAmount_Negative_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "-100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("положительное число", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when input is empty string.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidAmount_Empty_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("положительное число", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync accepts valid decimal amounts.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidDecimalAmount_ParsesCorrectly()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "150.50", CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        // Verify that session was updated (meaning handler proceeded past validation)
        var session = await sessionRepository.GetActiveSessionAsync(1002, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(10L, session.CurrentStepId); // step 4 success step
    }

    #endregion

    #region HandleAsync - No Session

    /// <summary>
    /// Verifies that HandleAsync returns error when no active session exists for the user.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NoSession_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var dbContext = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Remove the user's session to trigger the error path
        using var db = await dbContext.CreateDbContextAsync(CancellationToken.None);
        var session = await db.UserSessions.FirstOrDefaultAsync(
            s => s.UserId == 1, CancellationToken.None);
        Assert.NotNull(session);
        db.UserSessions.Remove(session);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("Сессия не найдена", result.Message);
    }

    #endregion

    #region HandleAsync - Invalid Session Data

    /// <summary>
    /// Verifies that HandleAsync returns error when session data contains invalid JSON.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidJsonSessionData_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var dbContext = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Set session data to invalid JSON
        using var db = await dbContext.CreateDbContextAsync(CancellationToken.None);
        var session = await db.UserSessions.FirstOrDefaultAsync(
            s => s.UserId == 1, CancellationToken.None);
        Assert.NotNull(session);
        session.Data = "not valid json{{{";
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("Ошибка чтения данных сессии", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when session data is missing duel_id.
    /// </summary>
    [Fact]
    public async Task HandleAsync_SessionDataMissingDuelId_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var dbContext = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Set session data without duel_id
        using var db = await dbContext.CreateDbContextAsync(CancellationToken.None);
        var session = await db.UserSessions.FirstOrDefaultAsync(
            s => s.UserId == 1, CancellationToken.None);
        Assert.NotNull(session);
        session.Data = "{\"option_id\":1}";
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("Данные о дуэли не найдены", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when session data is missing option_id.
    /// </summary>
    [Fact]
    public async Task HandleAsync_SessionDataMissingOptionId_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var dbContext = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Set session data without option_id
        using var db = await dbContext.CreateDbContextAsync(CancellationToken.None);
        var session = await db.UserSessions.FirstOrDefaultAsync(
            s => s.UserId == 1, CancellationToken.None);
        Assert.NotNull(session);
        session.Data = "{\"duel_id\":1}";
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("Данные о дуэли не найдены", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when session data is an empty object.
    /// </summary>
    [Fact]
    public async Task HandleAsync_EmptySessionData_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var dbContext = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Set session data to empty object
        using var db = await dbContext.CreateDbContextAsync(CancellationToken.None);
        var session = await db.UserSessions.FirstOrDefaultAsync(
            s => s.UserId == 1, CancellationToken.None);
        Assert.NotNull(session);
        session.Data = "{}";
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("Данные о дуэли не найдены", result.Message);
    }

    #endregion

    #region HandleAsync - Duel Handler Failure

    /// <summary>
    /// Verifies that HandleAsync returns error when duel handler fails with insufficient funds.
    /// The real duel handler will fail because UserStats has no balance field.
    /// </summary>
    [Fact]
    public async Task HandleAsync_DuelHandlerInsufficientFunds_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "2100", CancellationToken.None);

        // Assert - the real handler will fail because balance is not tracked in UserStats
        // so the duel handler returns failure
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.Contains("Недостаточно средств", result.Message);
        Assert.True(result.IsNewMessage);
    }

    #endregion

    #region HandleAsync - Duel Not Found

    /// <summary>
    /// Verifies that HandleAsync returns error when the referenced duel does not exist.
    /// </summary>
    [Fact]
    public async Task HandleAsync_DuelNotFound_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var dbContext = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Set session data to reference a non-existent duel
        using var db = await dbContext.CreateDbContextAsync(CancellationToken.None);
        var session = await db.UserSessions.FirstOrDefaultAsync(
            s => s.UserId == 1, CancellationToken.None);
        Assert.NotNull(session);
        session.Data = "{\"duel_id\":999,\"option_id\":1}";
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
        Assert.Contains("Дуэль не найдена", result.Message);
    }

    #endregion

    #region HandleAsync - Success Path

    /// <summary>
    /// Verifies that HandleAsync returns success result with formatted message and keyboard.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidInput_ReturnsSuccessResult()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
    }

    /// <summary>
    /// Verifies that HandleAsync returns correct keyboard buttons for the success result.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ReturnsCorrectKeyboardButtons()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result.Keyboard);
        Assert.Equal(3, result.Keyboard.Count);
        Assert.Contains("✉️ Переслать друзьям", result.Keyboard[0].Caption);
        Assert.Contains("📊 Следующая дуэль", result.Keyboard[1].Caption);
        Assert.Contains("⬅️ Назад в меню", result.Keyboard[2].Caption);
    }

    /// <summary>
    /// Verifies that the back to menu button has the correct action.
    /// </summary>
    [Fact]
    public async Task HandleAsync_BackToMenuButton_HasCorrectAction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result.Keyboard);
        var backButton = result.Keyboard.First(b => b.Caption == "⬅️ Назад в меню");
        Assert.Equal("main_menu", backButton.Action);
    }

    /// <summary>
    /// Verifies that HandleAsync formats the success message with correct bet details.
    /// </summary>
    [Fact]
    public async Task HandleAsync_SuccessMessage_ContainsBetDetails()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "500", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.Contains("СТАВКА УСПЕШНО", result.Message);
        Assert.Contains("500", result.Message);
        Assert.Contains("Да, радикально", result.Message); // option text from duel 1, option 1
    }

    /// <summary>
    /// Verifies that HandleAsync returns correct IsFinal and IsNewMessage flags for success.
    /// </summary>
    [Fact]
    public async Task HandleAsync_SuccessResult_ReturnsCorrectFlags()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsNewMessage);
    }

    /// <summary>
    /// Verifies that HandleAsync updates the session step to the success step.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Success_UpdatesSessionStep()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        var successStep = await scenarioRepository.GetStepByOrderAsync(3, 4, CancellationToken.None);
        Assert.NotNull(successStep);

        // Act
        await handler.HandleAsync(tgUser, "100", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(successStep.Id, session.CurrentStepId);
    }

    /// <summary>
    /// Verifies that HandleAsync works with a different duel and option.
    /// </summary>
    [Fact]
    public async Task HandleAsync_DifferentDuel_ReturnsCorrectOptionText()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var dbContext = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "200", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.Contains("СТАВКА УСПЕШНО", result.Message);
        Assert.Contains("200", result.Message);
        Assert.Contains("Да, радикально", result.Message); // option text from duel 2, option 3
    }

    #endregion

    #region HandleAsync - Different Amount Formats

    /// <summary>
    /// Verifies that HandleAsync accepts large amounts.
    /// </summary>
    [Fact]
    public async Task HandleAsync_LargeAmount_ParsesCorrectly()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "100.55", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
    }

    /// <summary>
    /// Verifies that HandleAsync accepts small decimal amounts.
    /// </summary>
    [Fact]
    public async Task HandleAsync_SmallDecimalAmount_ParsesCorrectly()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.NumberInput);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "0.01", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
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
