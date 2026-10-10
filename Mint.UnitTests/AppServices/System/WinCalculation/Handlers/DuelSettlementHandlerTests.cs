using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.UnitTests.AppServices.System.WinCalculation.Fixtures;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Handlers;

/// <summary>
/// Tests for <see cref="DuelSettlementHandler"/> that verify the settlement phase only:
/// payout plan creation and duel closing. Payout execution is covered by
/// <see cref="DuelSettlementHandlerExecutionTests"/>.
/// </summary>
public class DuelSettlementHandlerTests : IClassFixture<DuelSettlementHandlerFixture>, IDisposable
{
    /// <summary>
    /// Identifier of the seeded duel with an even vote split that produces a draw.
    /// </summary>
    private const long DrawDuelId = 6;

    private readonly DuelSettlementHandlerFixture _fixture;

    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelSettlementHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Settlement handler fixture.</param>
    public DuelSettlementHandlerTests(DuelSettlementHandlerFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    #region PlanDuelPayoutsAsync - Batch Processing

    /// <summary>
    /// Verifies that every expired active duel is settled and counted.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_WhenExpiredDuelsExist_ReturnsSettledCount()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        var result = await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert - expired active duels 1, 2 and the draw duel 6.
        Assert.Equal(3, result);

        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        var duel1 = await duelRepository.GetDuelByIdAsync(1, CancellationToken.None);
        var duel2 = await duelRepository.GetDuelByIdAsync(2, CancellationToken.None);

        Assert.NotNull(duel1);
        Assert.Equal(DuelStatus.Closed, duel1.Status);

        Assert.NotNull(duel2);
        Assert.Equal(DuelStatus.Closed, duel2.Status);
    }

    /// <summary>
    /// Verifies that zero is returned when there is nothing to settle.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_WhenNoExpiredDuels_ReturnsZero()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Close every expired active duel so that the batch is empty.
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using (var context = await factory.CreateDbContextAsync())
        {
            var expired = await context.Duels
                .Where(d => d.Status == DuelStatus.Active && d.ExpiresAt < DateTimeOffset.UtcNow)
                .ToListAsync();

            foreach (var duel in expired)
            {
                duel.Status = DuelStatus.Closed;
            }

            await context.SaveChangesAsync();
        }

        // Act
        var result = await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// Verifies that an already closed duel is never settled again.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldSkipAlreadyClosedDuel_WhenDuelIsClosed()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Duel 3 is expired but already closed in the seed.
        var duel3Payouts = await context.Payouts.CountAsync(p => p.DuelId == 3);
        Assert.Equal(0, duel3Payouts);
    }

    /// <summary>
    /// Verifies that a duel which is not expired yet stays untouched.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldSkipNotExpiredDuel_WhenDuelIsStillActive()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Duels 4 and 5 expire in the future, duel 5 even has votes.
        var notExpiredPayouts = await context.Payouts
            .CountAsync(p => p.DuelId == 4 || p.DuelId == 5);
        Assert.Equal(0, notExpiredPayouts);

        var duel4 = await context.Duels.SingleAsync(d => d.Id == 4);
        var duel5 = await context.Duels.SingleAsync(d => d.Id == 5);

        Assert.Equal(DuelStatus.Active, duel4.Status);
        Assert.Equal(DuelStatus.Active, duel5.Status);
    }

    /// <summary>
    /// Verifies that all expired active duels are settled in a single run.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldSettleEveryExpiredDuel_WhenSeveralExpiredDuelsExist()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        var result = await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert - duels 1, 2 and the seeded draw duel 6.
        Assert.Equal(3, result);

        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        var stillActive = await context.Duels
            .CountAsync(d => d.Status == DuelStatus.Active && d.ExpiresAt < DateTimeOffset.UtcNow);

        Assert.Equal(0, stillActive);
    }

    /// <summary>
    /// Verifies that the second run settles nothing because the duels are already closed.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldNotDuplicatePayouts_WhenRunTwice()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);
        var secondRunResult = await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(0, secondRunResult);

        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        Assert.Equal(5, await context.Payouts.CountAsync(p => p.DuelId == 1));

        
        Assert.Equal(7, await context.Payouts.CountAsync());
    }

    #endregion

    #region PlanDuelPayoutsAsync - Payout Amounts

    /// <summary>
    /// Verifies the prize distribution for a duel with a winning option.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldCreatePayouts_WhenDuelExpired()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();
        var payouts = await context.Payouts.Where(p => p.DuelId == 1).ToListAsync();

        // Duel 1: option 1 wins (2 votes: account 2 bet 500, account 3 bet 300).
        // Total pot = 1000, house cut = 50, prize pool = 950, win factor = 950/800 = 1.1875.
        // Account 2 payout = 500 * 1.1875 = 593.75; Account 3 payout = 300 * 1.1875 = 356.25.
        // Account 4 voted for option 2 (loser) - gets a zero amount placeholder.
        Assert.Equal(5, payouts.Count);
        Assert.Contains(payouts, p => p.CreditAccountId == long.MinValue && p.Amount == 0.0m);
        Assert.Contains(payouts, p => p.CreditAccountId == 2 && p.Amount == 593.75m);
        Assert.Contains(payouts, p => p.CreditAccountId == 3 && p.Amount == 356.25m);
    }

    /// <summary>
    /// Verifies that each winning vote produces exactly one payout record.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldCreatePayoutForEachWinningVote_WhenMultipleWinners()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();
        var payouts = await context.Payouts.Where(p => p.DuelId == 1).ToListAsync();

        // 3 voters, 2 winners (option 1) plus one zero amount placeholder for the loser.
        Assert.Equal(5, payouts.Count);
        Assert.Equal(4, payouts.Count(p => p.Amount > 0));
    }

    /// <summary>
    /// Verifies that a losing voter receives no real payout record.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldNotCreatePayoutsForLosingVoters_WhenSomeVotersLose()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Account 4 voted for option 2 which lost - the payout is planned for the placeholder account.
        var payoutForAccount4 = await context.Payouts
            .AnyAsync(p => p.DuelId == 1 && p.CreditAccountId == 4);
        Assert.False(payoutForAccount4);

        var placeholder = await context.Payouts.SingleAsync(p => p.DuelId == 1 && p.Amount == 0);
        Assert.Equal(long.MinValue, placeholder.CreditAccountId);
        Assert.Equal(long.MinValue, placeholder.DebitAccountId);
        Assert.Equal(BonusType.None, placeholder.BonusType);
    }

    /// <summary>
    /// Verifies that a duel without votes is closed without creating any payout.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldCloseDuelWithoutPayouts_WhenNoVotes()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Duel 2 has no votes - it should be closed with zero payouts.
        var duel2 = await context.Duels.SingleAsync(d => d.Id == 2);
        Assert.NotNull(duel2);
        Assert.Equal(DuelStatus.Closed, duel2.Status);

        var duel2Payouts = await context.Payouts.CountAsync(p => p.DuelId == 2);
        Assert.Equal(0, duel2Payouts);
    }

    #endregion

    #region PlanDuelPayoutsAsync - Payout Plan Fields

    /// <summary>
    /// Verifies that payout plans reference the votes they were derived from.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_PayoutRecords_ShouldHaveCorrectVoteIds()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Votes 1 and 2 are the winning votes for duel 1 (option 1).
        var payoutVoteIds = await context.Payouts
            .Where(p => p.DuelId == 1)
            .Select(p => p.VoteId)
            .ToListAsync();

        Assert.Contains(1, payoutVoteIds);
        Assert.Contains(2, payoutVoteIds);

        // Vote 3 (account 4, option 2) is a losing vote - its plan carries no vote reference.
        Assert.DoesNotContain(3, payoutVoteIds);
    }

    /// <summary>
    /// Verifies that payout plans stay pending until the execution phase.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_PayoutRecords_ShouldBePendingPlansWithoutTransaction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Settlement only creates plans - TransactionId must be null until the execution phase runs.
        var payouts = await context.Payouts.Where(p => p.DuelId == 1).ToListAsync();

        Assert.NotEmpty(payouts);
        Assert.All(payouts, p => Assert.Null(p.TransactionId));
        Assert.All(payouts, p => Assert.Null(p.ProcessedAt));
        Assert.All(payouts, p => Assert.Equal(PayoutStatus.Pending, p.Status));
    }

    /// <summary>
    /// Verifies that winning payout plans carry the system account, bonus type and description.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_PayoutRecords_ShouldCaptureDebitAccountAndBonusType()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // Account 1 is the system account used as the debit side of every real payout.
        var winningPayouts = await context.Payouts
            .Where(p => p.DuelId == 1 && p.Amount > 0)
            .ToListAsync();

        Assert.Equal(4, winningPayouts.Count);
        Assert.All(winningPayouts, p => Assert.Equal(1, p.DebitAccountId));
        Assert.All(winningPayouts, p => Assert.Equal(BonusType.Bet, p.BonusType));
        Assert.All(winningPayouts, p => Assert.Equal("Выплата за дуэль:1", p.Description));
    }

    /// <summary>
    /// Verifies that settlement does not touch user stats - that belongs to the execution phase.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldNotChangeUserStats_WhenPlanningPayouts()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        var before = await context.UserStats
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Select(s => new { s.UserId, s.RankPoints, s.TotalWins, s.TotalLosses, s.TotalDraws })
            .ToListAsync();

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var after = await context.UserStats
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Select(s => new { s.UserId, s.RankPoints, s.TotalWins, s.TotalLosses, s.TotalDraws })
            .ToListAsync();

        Assert.NotEmpty(before);
        Assert.Equal(before, after);
    }

    #endregion

    #region PlanDuelPayoutsAsync - Draw

    /// <summary>
    /// Verifies that a draw refunds every voter with the original stake.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldCreateRefundPayouts_WhenDuelIsDraw()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        // On a draw every voter is refunded their own stake: account 2 - 100, account 3 - 200.
        var payouts = await context.Payouts.Where(p => p.DuelId == DrawDuelId).ToListAsync();

        Assert.Equal(2, payouts.Count);
        Assert.Contains(payouts, p => p.CreditAccountId == 2 && p.Amount == 100m);
        Assert.Contains(payouts, p => p.CreditAccountId == 3 && p.Amount == 200m);
    }

    /// <summary>
    /// Verifies that refund payout plans are booked from the system account as refunds.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_RefundPayouts_ShouldUseSystemAccountAndRefundBonusType()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        var payouts = await context.Payouts.Where(p => p.DuelId == DrawDuelId).ToListAsync();

        Assert.NotEmpty(payouts);
        Assert.All(payouts, p => Assert.Equal(BonusType.Refund, p.BonusType));
        Assert.All(payouts, p => Assert.Equal(1, p.DebitAccountId));
        Assert.All(payouts, p => Assert.Equal($"Возврат за ничью:{DrawDuelId}", p.Description));
        Assert.All(payouts, p => Assert.Equal(PayoutStatus.Pending, p.Status));
        Assert.All(payouts, p => Assert.Null(p.TransactionId));
    }

    /// <summary>
    /// Verifies that a draw produces a plan for every vote, not only for one option.
    /// </summary>
    [Fact]
    public async Task PlanDuelPayoutsAsync_ShouldCreatePayoutForEveryVote_WhenDuelIsDraw()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.PlanDuelPayoutsAsync(CancellationToken.None);

        // Assert
        var factory = _currentScope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await factory.CreateDbContextAsync();

        var payoutVoteIds = await context.Payouts
            .Where(p => p.DuelId == DrawDuelId)
            .Select(p => p.VoteId)
            .ToListAsync();

        Assert.Equal(2, payoutVoteIds.Count);
        Assert.Contains(6, payoutVoteIds);
        Assert.Contains(7, payoutVoteIds);

        var drawDuel = await context.Duels.SingleAsync(d => d.Id == DrawDuelId);
        Assert.Equal(DuelStatus.Closed, drawDuel.Status);
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
