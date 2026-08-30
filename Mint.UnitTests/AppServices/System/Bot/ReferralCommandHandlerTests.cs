using HashidsNet;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;
using Moq;

namespace Mint.UnitTests.AppServices.System.Bot;

/// <summary>
/// Tests for <see cref="ReferralCommandHandler"/> using DI and EF Core.
/// </summary>
public class ReferralCommandHandlerTests : IClassFixture<ReferralCommandHandlerFixture>, IDisposable
{
    private readonly ReferralCommandHandlerFixture _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReferralCommandHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public ReferralCommandHandlerTests(ReferralCommandHandlerFixture fixture)
    {
        _fixture = fixture;
    }

    #region HandleAsync - Happy Path

    /// <summary>
    /// Verifies that HandleAsync returns the formatted referral message with keyboard.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidUser_ReturnsCommandResultWithMessage()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Referral);
        var user = ReferralCommandHandlerFixture.CreateMockUser(userId: 1002, firstName: "Alice", lastName: "Smith", userName: "alice.smith");

        // Act
        var result = await handler.HandleAsync(user, "referral", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.StartsWith("👥 **РЕФЕРАЛЬНАЯ ПРОГРАММА**", result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Equal(2, result.Keyboard.Count);
        Assert.Equal("✉️ Переслать другу", result.Keyboard[0].Caption);
        Assert.Contains("Присоединяйся к", result.Keyboard[0].Action);
        Assert.Equal("🔙 Назад в меню", result.Keyboard[1].Caption);
        Assert.False(result.IsFinal);
        Assert.False(result.IsNewMessage);
    }

    /// <summary>
    /// Verifies that HandleAsync uses zero referral count for a user without stats.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UserWithoutStats_UsesZeroReferralCount()
    {
        // Arrange - user 1003 (Charlie) is seeded without stats
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Referral);
        var hashids = _currentScope.ServiceProvider.GetRequiredService<IHashids>();
        var user = ReferralCommandHandlerFixture.CreateMockUser(userId: 1003, firstName: "Charlie", lastName: "Brown", userName: "charlie.brown");

        // Act
        var result = await handler.HandleAsync(user, "referral", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains(hashids.EncodeLong(1003), result.Message);
        Assert.Contains("0", result.Message);
    }

    #endregion

    #region HandleAsync - User Not Found

    /// <summary>
    /// Verifies that HandleAsync returns an error message when the user is not found in the database.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UserNotFound_ReturnsErrorMessage()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Referral);
        var user = ReferralCommandHandlerFixture.CreateMockUser(userId: 99999, firstName: "Ghost", lastName: "User", userName: "ghost.user");

        // Act
        var result = await handler.HandleAsync(user, "referral", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Пользователь не найден", result.Message);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
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
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Referral);

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.HandleAsync(null!, "referral", CancellationToken.None));
    }

    #endregion

    #region HandleAsync - Session Creation

    /// <summary>
    /// Verifies that HandleAsync creates a session with correct parameters.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CreatesSession_WithCorrectParameters()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.Referral);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();
        var user = ReferralCommandHandlerFixture.CreateMockUser(userId: 1002, firstName: "Alice", lastName: "Smith", userName: "alice.smith");

        var scenario = await scenarioRepository.GetScenarioByNameAsync("referral", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, CancellationToken.None);
        Assert.NotNull(step);

        // Act
        await handler.HandleAsync(user, "referral", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1002, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(scenario.Id, session.ScenarioId);
        Assert.Equal(step.Id, session.CurrentStepId);
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
