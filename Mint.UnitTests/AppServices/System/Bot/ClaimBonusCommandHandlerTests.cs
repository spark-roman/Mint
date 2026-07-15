using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Accounts.Repositories;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;
using Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;
using Telegram.Bot.Types;

namespace Mint.UnitTests.AppServices.System.Bot;

/// <summary>
/// Tests for <see cref="ClaimBonusCommandHandler"/> using DI and EF Core.
/// </summary>
public class ClaimBonusCommandHandlerTests : IClassFixture<ClaimBonusCommandHandlerFixtures>, IDisposable
{
    private readonly ClaimBonusCommandHandlerFixtures _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClaimBonusCommandHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public ClaimBonusCommandHandlerTests(ClaimBonusCommandHandlerFixtures fixture)
    {
        _fixture = fixture;
    }

    #region HandleAsync - Happy Path

    /// <summary>
    /// Verifies that HandleAsync returns success message with new balance for a valid user.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidUser_ReturnsSuccessWithBalance()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsNewMessage);
        Assert.False(result.IsFinal);
        Assert.Contains("Ежедневный бонус", result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Equal(3, result.Keyboard.Count);
    }

    /// <summary>
    /// Verifies that HandleAsync creates a bonus stats entry for a new user.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NewUser_CreatesBonusStats()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        var stats = await bonusStatsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(1, stats.CurrentDailyStreak);
        Assert.Equal(100m, stats.TotalDailyBonusesClaimed);
    }

    /// <summary>
    /// Verifies that HandleAsync creates a transaction for the daily bonus.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CreatesTransaction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<Mint.Database.Entities.Ledger.Transactions.Repositories.ITransactionRepository>();
        var tgUser = new User { Id = 1001, IsBot = false, FirstName = "Alice" };

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        var crediAccountId = 1;
        // Assert
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(crediAccountId, CancellationToken.None);
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);
        var transaction = transactions.First(t => t.BounusType == BonusType.Daily);
        Assert.Equal(100m, transaction.Amount);
    }

    /// <summary>
    /// Verifies that HandleAsync updates the user's account balance.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UpdatesAccountBalance()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var tgUser = new User { Id = 1002, IsBot = false, FirstName = "Bob" };

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        var balance = await accountRepository.GetUserBalanceAsync(1002, CancellationToken.None);
        Assert.Equal(1500.50m + 100.00m, balance); // 10000000 + 100
    }

    #endregion

    #region HandleAsync - Account Not Found

    /// <summary>
    /// Verifies that HandleAsync returns error message when user account is not found.
    /// </summary>
    [Fact]
    public async Task HandleAsync_AccountNotFound_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var tgUser = new User { Id = 99999, IsBot = false, FirstName = "Nobody" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsFinal);
        Assert.Contains("не удалось найти ваш аккаунт", result.Message);
    }

    #endregion

    #region HandleAsync - Bonus Not Eligible

    /// <summary>
    /// Verifies that HandleAsync returns error message when bonus is not eligible.
    /// </summary>
    [Fact]
    public async Task HandleAsync_BonusNotEligible_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var tgUser = new User { Id = 1003, IsBot = false, FirstName = "John" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsNewMessage);
        Assert.False(result.IsFinal);
    }

    #endregion

    #region HandleAsync - Already Applied

    /// <summary>
    /// Verifies that HandleAsync returns info message when bonus is already applied.
    /// </summary>
    [Fact]
    public async Task HandleAsync_BonusAlreadyApplied_ReturnsInfoMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var tgUser = new User { Id = 1003, IsBot = false, FirstName = "John" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsNewMessage);
        Assert.False(result.IsFinal);
    }

    #endregion

    #region HandleAsync - Weekly Streak (Day 7)

    /// <summary>
    /// Verifies that HandleAsync applies weekly streak bonus when streak reaches 7.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WeeklyStreak_ReturnsStreakBonusMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var tgUser = new User { Id = 1004, IsBot = false, FirstName = "Billy" };

        // Act
        var result = await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsFinal);
        Assert.Contains("Стрик 7 дней", result.Message);
        Assert.Contains("1,100", result.Message); // 100 daily + 1000 streak
    }

    /// <summary>
    /// Verifies that HandleAsync creates both daily and streak transactions for weekly streak.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WeeklyStreak_CreatesDailyAndStreakTransactions()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<Mint.Database.Entities.Ledger.Transactions.Repositories.ITransactionRepository>();
        var tgUser = new User { Id = 1004, IsBot = false, FirstName = "Billy" };

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        var crediAccountId = 4;

        // Assert
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(crediAccountId, CancellationToken.None);
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);
        
        var dailyTransaction = transactions.FirstOrDefault(t => t.BounusType == Mint.Common.Contracts.UserInteractive.Bonuses.BonusType.Daily);
        var streakTransaction = transactions.FirstOrDefault(t => t.BounusType == Mint.Common.Contracts.UserInteractive.Bonuses.BonusType.Streak);
        
        Assert.NotNull(dailyTransaction);
        Assert.Equal(100m, dailyTransaction.Amount);
        
        Assert.NotNull(streakTransaction);
        Assert.Equal(1000m, streakTransaction.Amount);
    }

    /// <summary>
    /// Verifies that HandleAsync updates account balance correctly for weekly streak.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WeeklyStreak_UpdatesAccountBalance()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var tgUser = new User { Id = 1004, IsBot = false, FirstName = "Billy" };

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        var balance = await accountRepository.GetUserBalanceAsync(1004, CancellationToken.None);
        Assert.Equal(4300.00m, balance); // 3200 + 100 daily + 1000 streak
    }

    /// <summary>
    /// Verifies that HandleAsync resets streak to 0 after weekly streak bonus.
    /// </summary>
    [Fact]
    public async Task HandleAsync_WeeklyStreak_ResetsStreakToZero()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        var tgUser = new User { Id = 1004, IsBot = false, FirstName = "Billy" };

        // Act
        await handler.HandleAsync(tgUser, "", CancellationToken.None);

        // Assert
        var stats = await bonusStatsRepository.GetStatsByUserIdAsync(1004, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(0, stats.CurrentDailyStreak); // Streak resets to 0 after day 7
        Assert.Equal(700m, stats.TotalDailyBonusesClaimed); // 600 + 100
        Assert.Equal(1000m, stats.TotalStreakBonusesClaimed); // 0 + 1000
    }

    #endregion

    #region HandleAsync - Null Input

    /// <summary>
    /// Verifies that HandleAsync throws ArgumentNullException when tgUser is null.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NullUser_ThrowsArgumentNullException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<ICommandHandler>(TgCommandType.ClaimBonus);

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.HandleAsync(null!, "", CancellationToken.None));
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
