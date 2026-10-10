using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database;
using Mint.Database.Entities.System.Payouts;
using Mint.UnitTests.AppServices.System.WinCalculation.Fixtures;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Handlers;

/// <summary>
/// Tests for <see cref="DuelSettlementHandler"/> that verify the execution phase
/// (<see cref="DuelSettlementHandler.ExecutePendingPayoutsAsync"/>). Uses the fixture with
/// EF Core in-memory database and DI, following the same approach as <see cref="DuelSettlementHandlerTests"/>.
/// </summary>
public class DuelSettlementHandlerExecutionTests : IClassFixture<DuelSettlementHandlerFixture>, IDisposable
{
    private readonly DuelSettlementHandlerFixture _fixture;

    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelSettlementHandlerExecutionTests"/> class.
    /// </summary>
    /// <param name="fixture">Settlement handler fixture.</param>
    public DuelSettlementHandlerExecutionTests(DuelSettlementHandlerFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    #region ExecutePendingPayoutsAsync - Batch Loop

    /// <summary>
    /// Verifies that an empty claim stops the run immediately.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenNoPendingPayouts_ReturnsZeroAndCreatesNothing()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Remove all pending payouts so there is nothing to execute.
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Payouts.RemoveRange(context.Payouts);
            await context.SaveChangesAsync();
        }

        // Act
        var result = await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// Verifies that a partial batch ends the loop after the first claim.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenClaimReturnsPartialBatch_StopsAfterFirstBatch()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Seeder has 2 pending payouts, batch size is 100 (from settings).
        // 2 < 100, so the loop stops after the first batch.

        // Act
        var result = await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, result);
    }

    #endregion

    #region ExecutePendingPayoutsAsync - Transaction

    /// <summary>
    /// Verifies that a bet payout is booked as a transaction built from the payout plan.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenBetPayout_CreatesTransactionFromPayoutFields()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        var transactions = await context.Transactions.Where(t => t.DebitAccountId == 1).ToListAsync();

        // Both seeded payouts debit from account 1.
        Assert.Equal(2, transactions.Count);
        Assert.Contains(transactions, t => t.CreditAccountId == 2 && t.Amount == 593.75m && t.BonusTypeId == (int)BonusType.Bet);
        Assert.Contains(transactions, t => t.CreditAccountId == 3 && t.Amount == 356.25m && t.BonusTypeId == (int)BonusType.Bet);
    }

    /// <summary>
    /// Verifies that a refund payout is booked as a transaction as well.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenRefundPayout_CreatesTransaction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Add a refund payout for account 2.
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Payouts.Add(new PayoutEntity
            {
                Id = 100,
                VoteId = 1,
                DuelId = 1,
                CreditAccountId = 2,
                Amount = 100m,
                Status = PayoutStatus.Pending,
                DebitAccountId = 1,
                BonusType = BonusType.Refund,
                Description = "Возврат за ничью:1",
                PlannedAt = DateTimeOffset.UtcNow.AddHours(-1)
            });
            await context.SaveChangesAsync();
        }

        // Act
        var result = await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.True(result >= 1);

        await using var verifyContext = await factory.CreateDbContextAsync();
        var refundTx = await verifyContext.Transactions
            .FirstOrDefaultAsync(t => t.BonusTypeId == (int)BonusType.Refund && t.CreditAccountId == 2);

        Assert.NotNull(refundTx);
        Assert.Equal("Возврат за ничью:1", refundTx.Description);
    }

    /// <summary>
    /// Verifies that a payout without a bonus type is completed without a transaction.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenNoneBonusType_DoesNotCreateTransaction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Remove all payouts and add only a None-type payout.
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Payouts.RemoveRange(context.Payouts);
            context.Payouts.Add(new PayoutEntity
            {
                Id = 200,
                VoteId = 1,
                DuelId = 1,
                CreditAccountId = 2,
                Amount = 0m,
                Status = PayoutStatus.Pending,
                DebitAccountId = 1,
                BonusType = BonusType.None,
                Description = "No bonus",
                PlannedAt = DateTimeOffset.UtcNow.AddHours(-1)
            });
            await context.SaveChangesAsync();
        }

        var txCountBefore = await GetTransactionCountAsync(factory);

        // Act
        var result = await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
        var txCountAfter = await GetTransactionCountAsync(factory);
        Assert.Equal(txCountBefore, txCountAfter);
    }

    /// <summary>
    /// Verifies that the completed payout is attached to the transaction that was just created.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_ShouldCompletePayoutWithCreatedTransactionId()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        var payout = await context.Payouts.SingleAsync(p => p.Id == 1);
        Assert.Equal(PayoutStatus.Completed, payout.Status);
        Assert.NotNull(payout.TransactionId);
        Assert.NotNull(payout.ProcessedAt);
    }

    #endregion

    #region ExecutePendingPayoutsAsync - User Stats

    /// <summary>
    /// Verifies that a bet payout adds rank points and one win to the account stats.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenBetPayout_UpdatesStatsWithWin()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Account 2 stats: UserId=2, initial RankPoints=100, TotalWins=5.
        // After bet payout: RankPoints=693.75, TotalWins=6.
        var stats = await context.UserStats.SingleAsync(s => s.UserId == 2);
        Assert.Equal(693.75m, stats.RankPoints);
        Assert.Equal(6, stats.TotalWins);
    }

    /// <summary>
    /// Verifies that a refund payout adds one draw and keeps rank points untouched.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenRefundPayout_UpdatesStatsWithDraw()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Remove all payouts and add only a refund payout for account 2.
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Payouts.RemoveRange(context.Payouts);
            context.Payouts.Add(new PayoutEntity
            {
                Id = 300,
                VoteId = 1,
                DuelId = 1,
                CreditAccountId = 2,
                Amount = 100m,
                Status = PayoutStatus.Pending,
                DebitAccountId = 1,
                BonusType = BonusType.Refund,
                Description = "Возврат за ничью:1",
                PlannedAt = DateTimeOffset.UtcNow.AddHours(-1)
            });
            await context.SaveChangesAsync();
        }

        // Act
        await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        await using var verifyContext = await factory.CreateDbContextAsync();
        var stats = await verifyContext.UserStats.SingleAsync(s => s.UserId == 2);

        // Refund: rank points stay at 100, wins stay at 5, draws increase by 1.
        Assert.Equal(100m, stats.RankPoints);
        Assert.Equal(5, stats.TotalWins);
        Assert.Equal(1, stats.TotalDraws);
    }

    /// <summary>
    /// Verifies that a payout without a bonus type adds one loss to the account stats.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenNoneBonusType_UpdatesStatsWithLoss()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Remove all payouts and add only a None-type payout.
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Payouts.RemoveRange(context.Payouts);
            context.Payouts.Add(new PayoutEntity
            {
                Id = 400,
                VoteId = 1,
                DuelId = 1,
                CreditAccountId = 2,
                Amount = 0m,
                Status = PayoutStatus.Pending,
                DebitAccountId = 1,
                BonusType = BonusType.None,
                Description = "No bonus",
                PlannedAt = DateTimeOffset.UtcNow.AddHours(-1)
            });
            await context.SaveChangesAsync();
        }

        // Act
        await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        await using var verifyContext = await factory.CreateDbContextAsync();
        var stats = await verifyContext.UserStats.SingleAsync(s => s.UserId == 2);

        // None: rank points stay at 100, wins stay at 5, losses increase by 1.
        Assert.Equal(100m, stats.RankPoints);
        Assert.Equal(5, stats.TotalWins);
        Assert.Equal(3, stats.TotalLosses);
    }

    /// <summary>
    /// Given a pending payout, when executing with real accounts,
    /// then the transaction is created and booked successfully.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenBatchHasSeveralPayouts_UpdatesStatsPerAccount()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Seeder has 2 payouts for accounts 2 and 3.

        // Act
        var result = await handler.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, result);

        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Account 2 (UserId=2): win +1, rankPoints +100
        var stats2 = await context.UserStats.SingleAsync(s => s.UserId == 2);
        Assert.Equal(6, stats2.TotalWins);
        Assert.Equal(693.75m, stats2.RankPoints);

        // Account 3 (UserId=3): win +1, rankPoints +100
        var stats3 = await context.UserStats.SingleAsync(s => s.UserId == 3);
        Assert.Equal(4, stats3.TotalWins);
        Assert.Equal(431.25m, stats3.RankPoints);
    }

    #endregion

    #region ExecutePendingPayoutsAsync - Failure Cases

    /// <summary>
    /// Verifies that a payout whose account has no stats fails the run.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_WhenUserStatsMissing_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Remove all payouts and add one for account 100 which has no stats.
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using (var context = await factory.CreateDbContextAsync())
        {
            context.Payouts.RemoveRange(context.Payouts);
            context.Payouts.Add(new PayoutEntity
            {
                Id = 500,
                VoteId = 1,
                DuelId = 1,
                CreditAccountId = 100,
                Amount = 100m,
                Status = PayoutStatus.Pending,
                DebitAccountId = 1,
                BonusType = BonusType.Bet,
                Description = "Выплата за дуэль:1",
                PlannedAt = DateTimeOffset.UtcNow.AddHours(-1)
            });
            await context.SaveChangesAsync();
        }

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.ExecutePendingPayoutsAsync(CancellationToken.None));

        Assert.Contains("User stats not found for account 100", exception.Message, StringComparison.Ordinal);
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

    /// <summary>
    /// Gets the total number of transactions in the database.
    /// </summary>
    /// <param name="factory">Database context factory.</param>
    /// <returns>Transaction count.</returns>
    private static async Task<int> GetTransactionCountAsync(IDbContextFactory<MintDbContext> factory)
    {
        await using var context = await factory.CreateDbContextAsync();
        return await context.Transactions.CountAsync();
    }
}
