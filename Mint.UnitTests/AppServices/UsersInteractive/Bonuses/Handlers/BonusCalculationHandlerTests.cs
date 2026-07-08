using Mint.Common.Contracts.Users;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Bonuses.Dto;
using Mint.App.Services.UserInteractive.Bonuses.Handlers;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;
using Mint.UnitTests.AppServices.UsersInteractive.Fixtures;
using Moq;

namespace Mint.UnitTests.AppServices.UsersInteractive.Bonuses.Handlers;

/// <summary>
/// Tests for <see cref="BonusCalculationHandler"/> using DI and EF Core.
/// </summary>
public class BonusCalculationHandlerTests : IClassFixture<BonusCalculationHandlerFixture>, IDisposable
{
    private readonly BonusCalculationHandlerFixture _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="BonusCalculationHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public BonusCalculationHandlerTests(BonusCalculationHandlerFixture fixture)
    {
        _fixture = fixture;
    }

    #region ApplyStartBonusAsync - Happy Path

    /// <summary>
    /// Verifies that ApplyStartBonusAsync applies bonus successfully for a new user.
    /// </summary>
    [Fact]
    public async Task ApplyStartBonusAsync_NewUser_ReturnsSuccess()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();

        // Act
        var result = await handler.ApplyStartBonusAsync(1002, (byte)AuthSystem.Tg, 3, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.False(result.AlreadyApplied);
        Assert.Equal(1000.00m, result.Amount);
        Assert.Contains("Стартовый бонус", result.Message);
    }

    /// <summary>
    /// Verifies that ApplyStartBonusAsync creates bonus stats for a new user.
    /// </summary>
    [Fact]
    public async Task ApplyStartBonusAsync_NewUser_CreatesBonusStats()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();

        // Act
        await handler.ApplyStartBonusAsync(1003, (byte)AuthSystem.Tg, 4, CancellationToken.None);

        // Assert
        var stats = await bonusStatsRepository.GetStatsByUserIdAsync(1003, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.True(stats.IsStartBonusClaimed);
        Assert.Equal(1000.00m, stats.TotalStartBonusesClaimed);
    }

    /// <summary>
    /// Verifies that ApplyStartBonusAsync creates a transaction for the start bonus.
    /// </summary>
    [Fact]
    public async Task ApplyStartBonusAsync_NewUser_CreatesTransaction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        // Act
        await handler.ApplyStartBonusAsync(1003, (byte)AuthSystem.Tg, 4, CancellationToken.None);

        // Assert
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(4, CancellationToken.None);
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);
        var transaction = transactions.First(t => t.BounusType == BonusType.Start);
        Assert.Equal(1000.00m, transaction.Amount);
        Assert.Equal(BonusType.Start, transaction.BounusType);
    }

    #endregion

    #region ApplyStartBonusAsync - Already Applied

    /// <summary>
    /// Verifies that ApplyStartBonusAsync returns AlreadyApplied for a user who already claimed the bonus.
    /// </summary>
    [Fact]
    public async Task ApplyStartBonusAsync_AlreadyClaimed_ReturnsAlreadyApplied()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();

        // Setup mock to deny start bonus (simulating already claimed)
        _fixture.BonusValidatorMock
            .Setup(v => v.CanApplyStartBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await handler.ApplyStartBonusAsync(1001, (byte)AuthSystem.Tg, 2, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.True(result.AlreadyApplied);
        Assert.Equal("Bonus already applied", result.Message);
        Assert.Equal(0, result.Amount);
    }

    #endregion

    #region ApplyDailyBonusAsync - Happy Path

    /// <summary>
    /// Verifies that ApplyDailyBonusAsync applies bonus successfully for a user with no stats.
    /// </summary>
    [Fact]
    public async Task ApplyDailyBonusAsync_NewUser_ReturnsSuccess()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();

        // Act
        var result = await handler.ApplyDailyBonusAsync(1003, (byte)AuthSystem.Tg, 4, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.False(result.IsStreakBonusApplied);
        Assert.Equal(100m, result.Amount);
        Assert.Contains("Ежедневный бонус", result.Message);
    }

    /// <summary>
    /// Verifies that ApplyDailyBonusAsync increments streak to 1 for a new user.
    /// </summary>
    [Fact]
    public async Task ApplyDailyBonusAsync_NewUser_IncrementsStreakTo1()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();

        // Setup mock to allow daily bonus
        _fixture.BonusValidatorMock
            .Setup(v => v.CanApplyDailyBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await handler.ApplyDailyBonusAsync(1003, (byte)AuthSystem.Tg, 4, CancellationToken.None);

        // Assert
        var stats = await bonusStatsRepository.GetStatsByUserIdAsync(1003, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(1, stats.CurrentDailyStreak);
        Assert.Equal(100m, stats.TotalDailyBonusesClaimed);
    }

    /// <summary>
    /// Verifies that ApplyDailyBonusAsync increments streak for an existing user.
    /// </summary>
    [Fact]
    public async Task ApplyDailyBonusAsync_ExistingUser_IncrementsStreak()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();

        // Act
        var result = await handler.ApplyDailyBonusAsync(1001, (byte)AuthSystem.Tg, 2, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(100m, result.Amount);
        Assert.False(result.IsStreakBonusApplied);

        var stats = await bonusStatsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(4, stats.CurrentDailyStreak); // 3 + 1
        Assert.Equal(400m, stats.TotalDailyBonusesClaimed); // 300 + 100
    }

    /// <summary>
    /// Verifies that ApplyDailyBonusAsync creates a transaction for the daily bonus.
    /// </summary>
    [Fact]
    public async Task ApplyDailyBonusAsync_NewUser_CreatesTransaction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        // Act
        await handler.ApplyDailyBonusAsync(1003, (byte)AuthSystem.Tg, 4, CancellationToken.None);

        // Assert
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(4, CancellationToken.None);
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);
        var transaction = transactions.First(t => t.BounusType == BonusType.Daily);
        Assert.Equal(100m, transaction.Amount);
        Assert.Equal(BonusType.Daily, transaction.BounusType);
    }

    #endregion

    #region ApplyDailyBonusAsync - Streak Bonus (Day 7)

    /// <summary>
    /// Verifies that ApplyDailyBonusAsync applies streak bonus when streak reaches 7.
    /// </summary>
    [Fact]
    public async Task ApplyDailyBonusAsync_OnDay7_AppliesStreakBonus()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        // Setup mock to allow daily bonus
        _fixture.BonusValidatorMock.Setup(v => v.CanApplyDailyBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Seed user with streak = 6
        var existingStats = await bonusStatsRepository.GetStatsByUserIdAsync(1002, (byte)AuthSystem.Tg, CancellationToken.None);
        var updateDto = new Mint.Database.Entities.UserInteractive.Bonuses.Dto.UserBonusStatsUpdateDto
        {
            ExternalUserId = 1002,
            IsStartBonusClaimed = existingStats!.IsStartBonusClaimed,
            CurrentDailyStreak = 6,
            TotalDailyBonusesClaimed = existingStats.TotalDailyBonusesClaimed,
            TotalStreakBonusesClaimed = existingStats.TotalStreakBonusesClaimed,
            TotalStartBonusesClaimed = existingStats.TotalStartBonusesClaimed,
            TotalReferralBonusesClaimed = existingStats.TotalReferralBonusesClaimed,
            TotalRankBonusClaimed = existingStats.TotalRankBonusClaimed,
            LastDailyClaimedAt = existingStats.LastDailyClaimedAt,
            NextDailyAvailableAt = DateTimeOffset.UtcNow.AddHours(-1),
            LastStreakClaimedAt = existingStats.LastStreakClaimedAt,
            LastRankBonusClaimedAt = existingStats.LastRankBonusClaimedAt
        };
        await bonusStatsRepository.UpdateStatsAsync(updateDto, CancellationToken.None);

        // Act
        var result = await handler.ApplyDailyBonusAsync(1002, (byte)AuthSystem.Tg, 3, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(result.IsStreakBonusApplied);
        Assert.Equal(1100m, result.Amount); // 100 daily + 1000 streak
        Assert.Contains("Стрик 7 дней", result.Message);
        Assert.Contains("100 ежедневных + 1000 за стрик", result.Message);

        // Verify streak reset to 0
        var stats = await bonusStatsRepository.GetStatsByUserIdAsync(1002, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(0, stats.CurrentDailyStreak); // Reset after 7
        Assert.Equal(1000m, stats.TotalStreakBonusesClaimed);

        // Verify streak transaction was created
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(3, CancellationToken.None);
        Assert.NotNull(transactions);
        var streakTransaction = transactions?.FirstOrDefault(t => t.BounusType == BonusType.Streak);
        Assert.NotNull(streakTransaction);
        Assert.Equal(1000m, streakTransaction.Amount);
    }

    #endregion

    #region ApplyDailyBonusAsync - Not Eligible

    /// <summary>
    /// Verifies that ApplyDailyBonusAsync returns failure when bonus is not eligible.
    /// </summary>
    [Fact]
    public async Task ApplyDailyBonusAsync_NotEligible_ReturnsFailure()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();

        // Setup mock to deny daily bonus
        _fixture.BonusValidatorMock
            .Setup(v => v.CanApplyDailyBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await handler.ApplyDailyBonusAsync(1001, (byte)AuthSystem.Tg, 2, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(0, result.Amount);
        Assert.False(result.IsStreakBonusApplied);
    }

    #endregion

    #region ApplyStartBonusAsync - Edge Cases

    /// <summary>
    /// Verifies that ApplyStartBonusAsync handles existing stats correctly by updating totals.
    /// </summary>
    [Fact]
    public async Task ApplyStartBonusAsync_ExistingStats_UpdatesTotalClaimed()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IBonusCalculationHandler>();
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();

        // Setup mock to allow start bonus (even though user 1001 already has it claimed)
        _fixture.BonusValidatorMock
            .Setup(v => v.CanApplyStartBonus(It.IsAny<UserBonusStatsDto?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await handler.ApplyStartBonusAsync(1001, (byte)AuthSystem.Tg, 2, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(1000.00m, result.Amount);

        var stats = await bonusStatsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(2000.00m, stats.TotalStartBonusesClaimed); // 1000 + 1000
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
