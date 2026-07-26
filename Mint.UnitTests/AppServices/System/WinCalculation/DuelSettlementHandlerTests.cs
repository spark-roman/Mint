using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Database;
using Mint.Database.Entities.Ledger.Transactions;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes;
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
        Assert.True(duel.IsClosed);
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
        Assert.True(duel.IsClosed);
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
        Assert.True(duel.IsClosed);

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
            new DuelEntity { Id = 1, IsClosed = true, Question = "", Description = "" },
            new DuelEntity { Id = 2, IsClosed = true, Question = "", Description = "" },
            new DuelEntity { Id = 4, IsClosed = true, Question = "", Description = "" });

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
        Assert.True(duel3.IsClosed);
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
        Assert.False(duel4.IsClosed);
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
        Assert.True(duel2.IsClosed);
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
        Assert.True(duel1.IsClosed);
        Assert.NotNull(duel2);
        Assert.True(duel2.IsClosed);
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
