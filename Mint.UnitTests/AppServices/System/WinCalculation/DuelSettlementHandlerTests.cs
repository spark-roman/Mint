using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
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

    #region SettleDuelAsync - Stats Updates for Losing Voters

    /// <summary>
    /// Verifies that settling a duel increments TotalLosses for losing voters.
    /// Account 4 (Charlie) voted for option 2 (losing), so TotalLosses should increase.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_LosingVoter_IncrementsTotalLosses()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - Account 4 (Charlie) voted for option 2 (losing option)
        var stats = await statsRepository.GetStatsByAccountIdAsync(4, CancellationToken.None);
        Assert.NotNull(stats);
        // TotalLosses should be incremented by 1
        Assert.Equal(5, stats.TotalLosses); // 4 + 1
    }

    /// <summary>
    /// Verifies that TotalWins is NOT incremented for losing voters.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_LosingVoter_TotalWinsUnchanged()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - Account 4 (Charlie) voted for losing option, TotalWins should not change
        var stats = await statsRepository.GetStatsByAccountIdAsync(4, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(2, stats.TotalWins); // unchanged
    }

    /// <summary>
    /// Verifies that RankPoints is NOT changed for losing voters.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_LosingVoter_RankPointsUnchanged()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - Account 4 (Charlie) voted for losing option, RankPoints should not change
        var stats = await statsRepository.GetStatsByAccountIdAsync(4, CancellationToken.None);
        Assert.NotNull(stats);
        Assert.Equal(50m, stats.RankPoints); // unchanged
    }

    /// <summary>
    /// Verifies that ReferralCount is NOT changed after settlement.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_ReferralCountUnchanged()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert - ReferralCount should not change for any voter
        var stats2 = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        var stats3 = await statsRepository.GetStatsByAccountIdAsync(3, CancellationToken.None);
        var stats4 = await statsRepository.GetStatsByAccountIdAsync(4, CancellationToken.None);

        Assert.NotNull(stats2);
        Assert.NotNull(stats3);
        Assert.NotNull(stats4);
        Assert.Equal(0, stats2.ReferralCount);
        Assert.Equal(0, stats3.ReferralCount);
        Assert.Equal(0, stats4.ReferralCount);
    }

    #endregion

    #region SettleDuelAsync - Stats Not Found

    /// <summary>
    /// Verifies that SettleDuelAsync throws when a losing voter has no stats record.
    /// Account 100 (Diana) has no stats in the seeder.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_LosingVoterNoStats_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Create a new duel with options
        var newDuelId = await duelRepository.CreateDuelAsync(
            new DuelCreateDto
            {
                CategoryId = 1,
                DuelType = DuelType.OpinionMatch,
                Question = "Test duel for no stats",
                Description = "Test",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(48),
                Options = new List<DuelOptionCreateDto>
                {
                    new() { OptionText = "Yes", OptionCode = "yes" },
                    new() { OptionText = "No", OptionCode = "no" }
                }
            },
            CancellationToken.None);

        // Get the options to find the correct IDs
        var duel = await duelRepository.GetDuelByIdAsync(newDuelId, CancellationToken.None);
        var options = duel!.Options.ToList();
        var winningOptionId = options[0].Id; // option 1 wins (more votes)
        var losingOptionId = options[1].Id;  // option 2 loses

        // Add votes: 2 votes for winning option (Account 2 + Account 3), 1 vote for losing option (Account 100)
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = newDuelId,
            AccountId = 2,
            ChosenOptionId = winningOptionId,
            BetAmount = 100m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = newDuelId,
            AccountId = 3,
            ChosenOptionId = winningOptionId,
            BetAmount = 200m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = newDuelId,
            AccountId = 100,
            ChosenOptionId = losingOptionId,
            BetAmount = 300m
        }, CancellationToken.None);

        // Act & Assert - Account 100 has no stats and loses, should throw
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.SettleDuelAsync(newDuelId, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that SettleDuelAsync throws when a winning voter has no stats record.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_WinningVoterNoStats_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Create a new duel with options
        var newDuelId = await duelRepository.CreateDuelAsync(
            new DuelCreateDto
            {
                CategoryId = 1,
                DuelType = DuelType.OpinionMatch,
                Question = "Test duel for no stats winner",
                Description = "Test",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(48),
                Options = new List<DuelOptionCreateDto>
                {
                    new() { OptionText = "Yes", OptionCode = "yes" },
                    new() { OptionText = "No", OptionCode = "no" }
                }
            },
            CancellationToken.None);

        // Get the options to find the correct IDs
        var duel = await duelRepository.GetDuelByIdAsync(newDuelId, CancellationToken.None);
        var options = duel!.Options.ToList();
        var winningOptionId = options[0].Id; // only option with votes -> wins

        // Only Account 100 votes for option 1 -> Account 100 wins
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = newDuelId,
            AccountId = 100,
            ChosenOptionId = winningOptionId,
            BetAmount = 100m
        }, CancellationToken.None);

        // Act & Assert - Account 100 has no stats and wins, should throw
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.SettleDuelAsync(newDuelId, CancellationToken.None));
    }

    #endregion

    #region SettleDuelAsync - Tie Scenario

    /// <summary>
    /// Verifies settlement when all voters choose the same option (unanimous win).
    /// All voters are winners, all get payouts and win stats.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_UnanimousVote_AllVotersWin()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Create a duel where all voters choose option 1
        var newDuelId = await duelRepository.CreateDuelAsync(
            new DuelCreateDto
            {
                CategoryId = 1,
                DuelType = DuelType.OpinionMatch,
                Question = "Unanimous duel",
                Description = "All voters on same option",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(48),
                Options = new List<DuelOptionCreateDto>
                {
                    new() { OptionText = "Yes", OptionCode = "yes" },
                    new() { OptionText = "No", OptionCode = "no" }
                }
            },
            CancellationToken.None);

        // Get the options to find the correct IDs
        var duel = await duelRepository.GetDuelByIdAsync(newDuelId, CancellationToken.None);
        var winningOptionId = duel!.Options.First().Id;

        // Both Account 2 and Account 3 vote for the winning option
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = newDuelId,
            AccountId = 2,
            ChosenOptionId = winningOptionId,
            BetAmount = 100m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = newDuelId,
            AccountId = 3,
            ChosenOptionId = winningOptionId,
            BetAmount = 200m
        }, CancellationToken.None);

        // Act
        await handler.SettleDuelAsync(newDuelId, CancellationToken.None);

        // Assert - both voters should have incremented TotalWins
        var stats2 = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        var stats3 = await statsRepository.GetStatsByAccountIdAsync(3, CancellationToken.None);

        Assert.NotNull(stats2);
        Assert.NotNull(stats3);
        Assert.Equal(6, stats2.TotalWins); // 5 + 1
        Assert.Equal(4, stats3.TotalWins); // 3 + 1
    }

    /// <summary>
    /// Verifies that in a tie scenario (multiple winning options), all tied options are processed.
    /// When tie occurs, each tied option is processed as winning, so each voter becomes
    /// a winner once (for their chosen option) and a loser once (for the other option).
    /// Net effect: TotalWins +1 and TotalLosses +1 for each voter.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_MultipleWinningOptions_AllProcessed()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Capture initial stats
        var initialStats2 = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        var initialStats3 = await statsRepository.GetStatsByAccountIdAsync(3, CancellationToken.None);
        Assert.NotNull(initialStats2);
        Assert.NotNull(initialStats3);

        // Create a duel with 3 options where options 1 and 2 are tied
        var newDuelId = await duelRepository.CreateDuelAsync(
            new DuelCreateDto
            {
                CategoryId = 1,
                DuelType = DuelType.OpinionMatch,
                Question = "Tie duel",
                Description = "Multiple winning options",
                ExpiresAt = DateTimeOffset.UtcNow.AddHours(48),
                Options = new List<DuelOptionCreateDto>
                {
                    new() { OptionText = "A", OptionCode = "a" },
                    new() { OptionText = "B", OptionCode = "b" },
                    new() { OptionText = "C", OptionCode = "c" }
                }
            },
            CancellationToken.None);

        // Get the options to find the correct IDs
        var duel = await duelRepository.GetDuelByIdAsync(newDuelId, CancellationToken.None);
        var options = duel!.Options.ToList();
        var optionAId = options[0].Id;
        var optionBId = options[1].Id;

        // 1 vote for option A, 1 vote for option B -> tie between options A and B
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = newDuelId,
            AccountId = 2,
            ChosenOptionId = optionAId,
            BetAmount = 100m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = newDuelId,
            AccountId = 3,
            ChosenOptionId = optionBId,
            BetAmount = 200m
        }, CancellationToken.None);

        // Act
        await handler.SettleDuelAsync(newDuelId, CancellationToken.None);

        // Assert - in a tie, each voter is winner once (for their option) and loser once (for the other)
        // Net effect: TotalWins +1 and TotalLosses +1 for each voter
        var stats2 = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        var stats3 = await statsRepository.GetStatsByAccountIdAsync(3, CancellationToken.None);

        Assert.NotNull(stats2);
        Assert.NotNull(stats3);
        Assert.Equal(initialStats2.TotalWins + 1, stats2.TotalWins);
        Assert.Equal(initialStats2.TotalLosses + 1, stats2.TotalLosses);
        Assert.Equal(initialStats3.TotalWins + 1, stats3.TotalWins);
        Assert.Equal(initialStats3.TotalLosses + 1, stats3.TotalLosses);
    }

    #endregion

    #region SettleExpiredDuelsAsync - Verification

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync settles only expired duels and counts them correctly.
    /// Duels 1 and 2 are expired, duel 4 is not expired.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_OnlyExpiredSettled_CountCorrect()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act
        var settledCount = await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, settledCount); // duels 1 and 2 are expired

        var duel1 = await duelRepository.GetDuelByIdAsync(1, CancellationToken.None);
        var duel2 = await duelRepository.GetDuelByIdAsync(2, CancellationToken.None);
        var duel4 = await duelRepository.GetDuelByIdAsync(4, CancellationToken.None);

        Assert.NotNull(duel1);
        Assert.NotNull(duel2);
        Assert.NotNull(duel4);
        Assert.Equal(DuelStatus.Closed, duel1.Status);
        Assert.Equal(DuelStatus.Closed, duel2.Status);
        Assert.Equal(DuelStatus.Active, duel4.Status); // not expired, should remain active
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync does not settle already closed duels.
    /// Duel 3 is already closed, should not be counted in settled.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_AlreadyClosedDuel_NotCounted()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        var settledCount = await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert - only expired active duels are settled (1 and 2), not closed duel 3
        Assert.Equal(2, settledCount);
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync updates stats for all settled duels.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_UpdatesStatsForAllSettledDuels()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert - stats should be updated for duel 1 voters
        var stats2 = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        var stats3 = await statsRepository.GetStatsByAccountIdAsync(3, CancellationToken.None);
        var stats4 = await statsRepository.GetStatsByAccountIdAsync(4, CancellationToken.None);

        Assert.NotNull(stats2);
        Assert.NotNull(stats3);
        Assert.NotNull(stats4);
        Assert.Equal(6, stats2.TotalWins); // 5 + 1 (winner)
        Assert.Equal(4, stats3.TotalWins); // 3 + 1 (winner)
        Assert.Equal(5, stats4.TotalLosses); // 4 + 1 (loser)
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync creates no transactions for duel without votes.
    /// Duel 2 has no votes, should be closed without transactions.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_DuelWithoutVotes_NoTransactions()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        await handler.SettleExpiredDuelsAsync(CancellationToken.None);

        // Assert
        using var scope = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var transactionsForDuel2 = context.Transactions
            .Where(t => t.Description!.Contains(":2"))
            .ToList();

        Assert.Empty(transactionsForDuel2);
    }

    #endregion

    #region SettleDuelAsync - Cancellation

    /// <summary>
    /// Verifies that SettleDuelAsync respects cancellation token.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_CancellationToken_Respected()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.SettleDuelAsync(1, cts.Token));
    }

    /// <summary>
    /// Verifies that SettleExpiredDuelsAsync respects cancellation token.
    /// </summary>
    [Fact]
    public async Task SettleExpiredDuelsAsync_CancellationToken_Respected()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.SettleExpiredDuelsAsync(cts.Token));
    }

    #endregion

    #region SettleDuelAsync - Multiple Duels Settlement

    /// <summary>
    /// Verifies that settling two duels in sequence updates stats correctly for both.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_SequentialSettlement_UpdatesStatsCorrectly()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act - settle duel 1 first
        await handler.SettleDuelAsync(1, CancellationToken.None);

        // Assert after first settlement
        var statsAfterFirst = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        Assert.NotNull(statsAfterFirst);
        Assert.Equal(6, statsAfterFirst.TotalWins);

        // Act - settle duel 2 (no votes, just closes)
        await handler.SettleDuelAsync(2, CancellationToken.None);

        // Assert - stats should not change for duel 2 (no votes)
        var statsAfterSecond = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        Assert.NotNull(statsAfterSecond);
        Assert.Equal(6, statsAfterSecond.TotalWins); // unchanged
    }

    /// <summary>
    /// Verifies that closing a duel without votes does not affect any stats.
    /// </summary>
    [Fact]
    public async Task SettleDuelAsync_NoVotes_NoStatsChanges()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Capture initial stats
        var initialStats2 = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        var initialStats3 = await statsRepository.GetStatsByAccountIdAsync(3, CancellationToken.None);

        // Act - duel 2 has no votes
        await handler.SettleDuelAsync(2, CancellationToken.None);

        // Assert - stats should be unchanged
        var finalStats2 = await statsRepository.GetStatsByAccountIdAsync(2, CancellationToken.None);
        var finalStats3 = await statsRepository.GetStatsByAccountIdAsync(3, CancellationToken.None);

        Assert.NotNull(initialStats2);
        Assert.NotNull(initialStats3);
        Assert.NotNull(finalStats2);
        Assert.NotNull(finalStats3);
        Assert.Equal(initialStats2.TotalWins, finalStats2.TotalWins);
        Assert.Equal(initialStats2.TotalLosses, finalStats2.TotalLosses);
        Assert.Equal(initialStats3.TotalWins, finalStats3.TotalWins);
        Assert.Equal(initialStats3.TotalLosses, finalStats3.TotalLosses);
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
