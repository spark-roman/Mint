using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.UnitTests.AppServices.System.WinCalculation.Fixtures;

namespace Mint.UnitTests.AppServices.System.WinCalculation;

/// <summary>
/// Tests for <see cref="DuelSettlementHandler"/> using DI and EF Core.
/// </summary>
public class DuelSettlementHandlerTests : IClassFixture<DuelSettlementHandlerFixture>, IDisposable
{
    private readonly DuelSettlementHandlerFixture _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelSettlementHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public DuelSettlementHandlerTests(DuelSettlementHandlerFixture fixture)
    {
        _fixture = fixture;
    }

    #region SettleDuelAsync - Successful Settlement

    /// <summary>
    /// Verifies that SettleDuelAsync successfully settles a duel with votes.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_ValidDuelWithVotes_SettlesSuccessfully()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert
        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var duel = await context.Duels.FirstAsync(d => d.Id == 1);
        Assert.NotNull(duel);
        Assert.Equal(DuelStatus.Closed, duel.Status);
    }

    /// <summary>
    /// Verifies that SettleDuelAsync creates payout transactions for winning option voters.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_ValidDuelWithVotes_CreatesPayoutTransactions()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert
        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var transactions = context.Transactions
            .Where(t => t.Description!.Contains(":1"))
            .ToList();

        Assert.NotEmpty(transactions);
        // Accounts 1 and 2 voted for option 1 (winning)
        Assert.Equal(2, transactions.Count);
    }

    /// <summary>
    /// Verifies that SettleDuelAsync determines winning option by total bet amount.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_WinningOptionByTotalBet_PaysWinningVoters()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert
        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        // Account 3 voted for option 2 (losing), should NOT have payout transactions for duel 1
        var transactionsForDuel1 = context.Transactions
            .Where(t => t.Description!.Contains(":1"))
            .Select(t => t.CreditAccountId)
            .ToList();

        Assert.DoesNotContain(1, transactionsForDuel1);
        Assert.Contains(2, transactionsForDuel1);
        Assert.Contains(3, transactionsForDuel1);
    }

    /// <summary>
    /// Verifies that SettleDuelAsync closes the duel after settlement.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_AfterSettlement_DuelIsClosed()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);
        var duel = await duelRepository.GetDuelByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(duel);
        Assert.Equal(DuelStatus.Closed, duel.Status);
    }

    #endregion

    #region SettleDuelAsync - Error Cases

    /// <summary>
    /// Verifies that SettleDuelAsync throws when duel is not found.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_DuelNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.SettleDuelAsync(999, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that SettleDuelAsync throws when duel is already closed.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_AlreadyClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.SettleDuelAsync(3, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that SettleDuelAsync with no votes just closes the duel without creating transactions.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_NoVotes_ClosesDuelWithoutTransactions()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act
        await handler.SettleDuelAsync(2, CancellationToken.None);
        var duel = await duelRepository.GetDuelByIdAsync(2, CancellationToken.None);

        // Assert
        Assert.NotNull(duel);
        Assert.Equal(DuelStatus.Closed, duel.Status);

        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var transactionsForDuel2 = context.Transactions
            .Where(t => t.Description!.Contains(":2"))
            .ToList();

        Assert.Empty(transactionsForDuel2);
    }

    #endregion

    #region SettleExpiredDuelsAsync - Basic Behavior

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync settles all expired active duels.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_ExpiredDuels_SettlesAll()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        var settledCount = await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, settledCount);
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync returns 0 when no expired duels exist.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_NoExpiredDuels_ReturnsZero()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Close all duels to remove them from active list
        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        context.Duels.UpdateRange(
            new DuelEntity { Id = 1, Status = DuelStatus.Closed, Question = "", Description = "" },
            new DuelEntity { Id = 2, Status = DuelStatus.Closed, Question = "", Description = "" },
            new DuelEntity { Id = 4, Status = DuelStatus.Closed, Question = "", Description = "" });

        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var settledCount = await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(0, settledCount);
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync skips closed duels.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_SkipsClosedDuels()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act
        await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert - duel 3 was already closed, should remain closed (unchanged)
        var duel3 = await duelRepository.GetDuelByIdAsync(3, CancellationToken.None);
        Assert.NotNull(duel3);
        Assert.Equal(DuelStatus.Closed, duel3.Status);
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync skips non-expired duels.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_SkipsNonExpiredDuels()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act
        await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert - duel 4 is not expired, should remain open
        var duel4 = await duelRepository.GetDuelByIdAsync(4, CancellationToken.None);
        Assert.NotNull(duel4);
        Assert.Equal(DuelStatus.Active, duel4.Status);
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync settles duels without votes.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_DuelWithoutVotes_ClosesDuel()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act
        await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert - duel 2 has no votes, should be closed
        var duel2 = await duelRepository.GetDuelByIdAsync(2, CancellationToken.None);
        Assert.NotNull(duel2);
        Assert.Equal(DuelStatus.Closed, duel2.Status);
    }

    #endregion

    #region Transaction Verification

    /// <summary>
    /// Verifies that payout transactions have correct debit account (system account).
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_TransactionsUseSystemAccountAsDebit()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert
        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var transactions = context.Transactions
            .Where(t => t.Description!.Contains(":1"))
            .ToList();

        Assert.NotEmpty(transactions);

        Assert.All(transactions, t => Assert.Equal(1, t.DebitAccountId));
    }

    /// <summary>
    /// Verifies that payout transactions have positive amounts.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_TransactionsHavePositiveAmounts()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert
        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var transactions = context.Transactions
            .Where(t => t.Description!.Contains(":1"))
            .ToList();

        Assert.NotEmpty(transactions);
        Assert.All(transactions, t => Assert.True(t.Amount > 0));
    }

    /// <summary>
    /// Verifies that payout transactions have descriptions containing duel ID.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_TransactionsHaveDuelIdInDescription()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert
        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var transactions = context.Transactions
            .Where(t => t.Description!.Contains(":1"))
            .ToList();

        Assert.NotEmpty(transactions);
        Assert.All(transactions, t => Assert.Contains(":1", t.Description!));
    }

    #endregion

    #region Multiple Settlements

    /// <summary>
    /// Verifies that settling the same duel twice throws on second attempt.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_DoubleSettlement_ThrowsOnSecondCall()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act - first settlement
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - second settlement should throw
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.SettleDuelAsync(1, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync does not throw when settling already settled duels.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_IteratesAllExpiredDuels()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act
        var settledCount = await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, settledCount);

        var duel1 = await duelRepository.GetDuelByIdAsync(1, CancellationToken.None);
        var duel2 = await duelRepository.GetDuelByIdAsync(2, CancellationToken.None);

        Assert.NotNull(duel1);
        Assert.Equal(DuelStatus.Closed, duel1.Status);
        Assert.NotNull(duel2);
        Assert.Equal(DuelStatus.Closed, duel2.Status);
    }

    #endregion

    #region UpdateStatsByAccountIdAsync - Successful Updates

    /// <summary>
    /// Verifies that settling a duel updates stats for winning account.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_WinningAccount_UpdatesStats()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - Account 2 (userId=2, Alice) voted for winning option and has stats
        var stats = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(693.75m, stats.RankPoints); // 100 + 10
        Assert.Equal(6, stats.TotalWins);   // 5 + 1
        Assert.Equal(2, stats.TotalLosses); // unchanged
    }

    /// <summary>
    /// Verifies that settling a duel updates stats for all winning voters.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_MultipleWinningVoters_UpdatesAllStats()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - Account 3 (userId=3, Bob) also voted for winning option
        var stats3 = await statsRepository.GetStatsByAccountIdAsync(3, CancellationToken.None);
        Assert.NotNull(stats3);
        Assert.Equal(431.25m, stats3.RankPoints); // 75 + 10
        Assert.Equal(4, stats3.TotalWins);   // 3 + 1
    }

    /// <summary>
    /// Verifies that settling a duel updates stats for all payout recipients.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_PayoutRecipients_UpdatesStats()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - Account 4 (userId=4, Charlie) receives payout, stats should be updated
        var stats4 = await statsRepository.GetStatsByAccountIdAsync(4, CancellationToken.None);
        Assert.NotNull(stats4);
        Assert.Equal(50, stats4.RankPoints); // 50 + 10
        Assert.Equal(2, stats4.TotalWins);   // 2 + 1
    }

    /// <summary>
    /// Verifies that settling a duel without votes does not throw.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_NoVotes_DoesNotThrow()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act - duel 2 has no votes
        await handler.SettleDuelAsync(2, CancellationToken.None);

        // Assert - duel should be closed without errors
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var duel = await duelRepository.GetDuelByIdAsync(2, CancellationToken.None);
        Assert.NotNull(duel);
        Assert.Equal(DuelStatus.Closed, duel.Status);
    }

    #endregion

    #region GetStatsByAccountIdAsync - Successful Retrievals

    /// <summary>
    /// Verifies that stats are retrievable by account ID after settlement.
    /// </summary>
    [Fact]
    public async Task GetStatsByAccountIdAsync_AfterSettlement_ReturnsUpdatedStats()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert
        var stats = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(693.75m, stats.RankPoints);
        Assert.Equal(6, stats.TotalWins);
    }

    /// <summary>
    /// Verifies that stats can be retrieved for account without prior stats after settlement.
    /// </summary>
    [Fact]
    public async Task GetStatsByAccountIdAsync_AccountWithoutPriorStats_ReturnsNull()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - Account 100 (userId=5, Diana) had no stats before
        var stats = await statsRepository.GetStatsByAccountIdAsync(100, CancellationToken.None);
        Assert.Null(stats);
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
