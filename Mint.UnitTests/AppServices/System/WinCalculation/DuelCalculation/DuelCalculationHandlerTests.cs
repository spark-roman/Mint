using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.UnitTests.AppServices.System.WinCalculation.Fixtures;

namespace Mint.UnitTests.AppServices.System.WinCalculation.DuelCalculation;

/// <summary>
/// Tests for <see cref="DuelCalculationHandler"/> using DI and EF Core In-Memory.
/// </summary>
public class DuelCalculationHandlerTests : IClassFixture<DuelCalculationHandlerFixture>, IDisposable
{
    private readonly DuelCalculationHandlerFixture _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelCalculationHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public DuelCalculationHandlerTests(DuelCalculationHandlerFixture fixture)
    {
        _fixture = fixture;
    }

    #region CalculateResultAsync - Successful Calculation

    /// <summary>
    /// Verifies that CalculateResultAsync returns correct result for a duel with votes.
    /// Duel 1: 3 votes (option 1: 500+100=600, option 2: 300), winning option = 1.
    /// Total pot = 900, house cut = 45 (5%), prize pool = 855.
    /// Win factor = 855/600 = 1.425.
    /// Alice payout = 500 * 1.425 = 712.5
    /// Bob payout = 100 * 1.425 = 142.5
    /// Charlie payout = null (lost)
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_ValidDuelWithVotes_ReturnsCorrectResult()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.DuelId);
        Assert.Equal((int)DuelType.OpinionMatch, result.DuelType);
        Assert.Equal(1, result.WinningOptionId);
        Assert.Equal(900m, result.TotalPot);
        Assert.Equal(45m, result.HouseCut);
        Assert.Equal(855m, result.PrizePool);
    }

    /// <summary>
    /// Verifies that winning voters receive payout instructions with correct amounts.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_WinningVoters_ReceivePayoutInstructions()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        var winningResults = result.VoteResults.Where(v => v.PayoutInstruction != null).ToList();
        Assert.Equal(2, winningResults.Count);

        // Alice (AccountId=2, bet=500): payout = 500 * 1.425 = 712.5
        var aliceResult = winningResults.First(v => v.VoteAccountId == 2);
        Assert.Equal(712.5m, aliceResult.PayoutInstruction!.Amount);

        // Bob (AccountId=3, bet=100): payout = 100 * 1.425 = 142.5
        var bobResult = winningResults.First(v => v.VoteAccountId == 3);
        Assert.Equal(142.5m, bobResult.PayoutInstruction!.Amount);
    }

    /// <summary>
    /// Verifies that losing voters receive null payout instructions.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_LosingVoters_ReceiveNullPayout()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        var losingResults = result.VoteResults.Where(v => v.PayoutInstruction == null).ToList();
        Assert.Single(losingResults);
        Assert.Equal(4, losingResults.First().VoteAccountId); // Charlie
    }

    /// <summary>
    /// Verifies that payout instructions use the system account as debit account.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_PayoutInstructionsUseSystemAccountAsDebit()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        Assert.All(result.VoteResults, v =>
        {
            if (v.PayoutInstruction != null)
            {
                Assert.Equal(1, v.PayoutInstruction.DebitAccountId); // system account
            }
        });
    }

    /// <summary>
    /// Verifies that payout instructions credit the correct account.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_PayoutInstructionsCreditCorrectAccount()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        var payouts = result.VoteResults.Where(v => v.PayoutInstruction != null).ToList();
        Assert.Equal(2, payouts.Count);
        Assert.Contains(payouts, v => v.PayoutInstruction!.CreditAccountId == 2); // Alice
        Assert.Contains(payouts, v => v.PayoutInstruction!.CreditAccountId == 3); // Bob
    }

    /// <summary>
    /// Verifies that payout descriptions contain the duel ID.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_PayoutDescriptionsContainDuelId()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        Assert.All(result.VoteResults, v =>
        {
            if (v.PayoutInstruction != null)
            {
                Assert.Contains(":1", v.PayoutInstruction.Description);
            }
        });
    }

    /// <summary>
    /// Verifies that all votes are included in the result, both winning and losing.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_AllVotesIncludedInResults()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.VoteResults.Count);
        Assert.Contains(result.VoteResults, v => v.VoteAccountId == 2); // Alice
        Assert.Contains(result.VoteResults, v => v.VoteAccountId == 3); // Bob
        Assert.Contains(result.VoteResults, v => v.VoteAccountId == 4); // Charlie
    }

    /// <summary>
    /// Verifies correct calculation when all votes are on the winning option (no losers).
    /// Duel 4: 1 vote (AccountId=2, bet=500), winning option = 1.
    /// Total pot = 500, house cut = 25, prize pool = 475.
    /// Win factor = 475/500 = 0.95.
    /// Payout = 500 * 0.95 = 475.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_AllVotesWinning_NoLosingVoters()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 4;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(4, result.DuelId);
        Assert.Equal(500m, result.TotalPot);
        Assert.Equal(25m, result.HouseCut);
        Assert.Equal(475m, result.PrizePool);
        Assert.Single(result.VoteResults);
        Assert.NotNull(result.VoteResults.First().PayoutInstruction);
        Assert.Equal(475m, result.VoteResults.First().PayoutInstruction!.Amount);
    }

    /// <summary>
    /// Verifies calculation with multiple winning voters receiving proportional payouts.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_WinningOptionHasMultipleVoters_AllGetPayouts()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        var winningPayouts = result.VoteResults.Where(v => v.PayoutInstruction != null).ToList();
        Assert.Equal(2, winningPayouts.Count);
        Assert.All(winningPayouts, p => Assert.True(p.PayoutInstruction!.Amount > 0));
    }

    #endregion

    #region CalculateResultAsync - Error Cases

    /// <summary>
    /// Verifies that CalculateResultAsync throws when duel is not found.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_DuelNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 999;
        var winningOptionId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that CalculateResultAsync throws when duel is already closed.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_AlreadyClosed_ThrowsInvalidOperationException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 3;
        var winningOptionId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that CalculateResultAsync returns empty result when there are no votes for the duel.
    /// Duel 2 has no votes.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_NoVotes_ReturnsEmptyResult()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        // Act
        var result = await handler.CalculateResultAsync(2, 1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.DuelId);
        Assert.Equal(0, result.TotalPot);
        Assert.Equal(0, result.HouseCut);
        Assert.Empty(result.VoteResults);
    }

    #endregion

    #region CalculateResultAsync - Edge Cases

    /// <summary>
    /// Verifies correct calculation with different bet amounts on winning vs losing options.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_DifferentBetAmounts_CorrectlyCalculates()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act - option 1 wins (2 votes vs 1 vote for option 2)
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.WinningOptionId);
        Assert.Equal(900m, result.TotalPot); // 500 + 100 + 300
        Assert.Equal(45m, result.HouseCut); // 5% of 900
        Assert.Equal(855m, result.PrizePool);
    }

    /// <summary>
    /// Verifies that house cut is always 5% of total pot.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_HouseCutIsAlwaysFivePercent()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        var expectedHouseCut = 900m * 0.05m; // 45
        Assert.Equal(expectedHouseCut, result.HouseCut);
    }

    /// <summary>
    /// Verifies that prize pool equals total pot minus house cut.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_PrizePoolEqualsTotalPotMinusHouseCut()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        Assert.Equal(result.TotalPot - result.HouseCut, result.PrizePool);
    }

    /// <summary>
    /// Verifies that win factor is correctly calculated as prizePool / winningTotal.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_WinFactorCorrectlyCalculated()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        // winningTotal = 500 + 100 = 600
        // prizePool = 855
        // winFactor = 855 / 600 = 1.425
        // Alice: 500 * 1.425 = 712.5
        var alicePayout = result.VoteResults.First(v => v.VoteAccountId == 2).PayoutInstruction!.Amount;
        Assert.Equal(712.5m, alicePayout);
    }

    /// <summary>
    /// Verifies that payout amounts are proportional to bet amounts.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_PayoutsProportionalToBets()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        var alicePayout = result.VoteResults.First(v => v.VoteAccountId == 2).PayoutInstruction!.Amount; // 500 bet
        var bobPayout = result.VoteResults.First(v => v.VoteAccountId == 3).PayoutInstruction!.Amount;   // 100 bet
        // Alice bet 5x more than Bob, so Alice payout should be 5x Bob payout
        Assert.Equal(5m * bobPayout, alicePayout);
    }

    /// <summary>
    /// Verifies that payout amounts are greater than bet amounts (due to winning opponents' money).
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_PayoutExceedsBetAmount()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;
        var winningOptionId = 1;

        // Act
        var result = await handler.CalculateResultAsync(duelId, winningOptionId, CancellationToken.None);

        // Assert
        var alicePayout = result.VoteResults.First(v => v.VoteAccountId == 2).PayoutInstruction!.Amount;
        Assert.True(alicePayout > 500m); // Alice bet 500, should receive more

        var bobPayout = result.VoteResults.First(v => v.VoteAccountId == 3).PayoutInstruction!.Amount;
        Assert.True(bobPayout > 100m); // Bob bet 100, should receive more
    }

    #endregion

    #region CalculateWinningOptionIdAsync - Successful Calculation

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns the correct winning option for OpinionMatch.
    /// Duel 1: option 1 has 2 votes, option 2 has 1 vote -> winner = [1].
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_OpinionMatch_ReturnsWinningOption()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(duelId, DuelType.OpinionMatch, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(1, result);
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns empty list for non-matching duel type.
    /// There is no rule for FactPrediction in the test setup.
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_NonMatchingRule_ReturnsEmptyList()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 1;

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(duelId, DuelType.FactPrediction, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns the correct winning option for duel 4.
    /// Duel 4: single vote on option 1 -> winner = [1].
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_SingleVote_ReturnsCorrectOption()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 4;

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(duelId, DuelType.OpinionMatch, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(1, result);
    }

    #endregion

    #region CalculateWinningOptionIdAsync - Edge Cases

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns winning option for duel 5.
    /// Duel 5: option 1 has 2 votes, option 2 has 1 vote -> winner = [1].
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_MultipleVotes_ReturnsWinningOption()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 5;

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(duelId, DuelType.OpinionMatch, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(1, result);
    }

    [Fact]
    public async Task CalculateWinningOptionIdAsync_MultipleEqualVotes_Returns2WinningOptions()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 6;

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(duelId, DuelType.OpinionMatch, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(11, result);
        Assert.Contains(12, result);
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns empty for duel with no votes.
    /// Duel 2 has no votes.
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_NoVotes_ReturnsEmptyList()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 2;

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(duelId, DuelType.OpinionMatch, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns empty for non-existent duel.
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_NonExistentDuel_ReturnsEmptyList()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var duelId = 999;

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(duelId, DuelType.OpinionMatch, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    #endregion

    #region Cancellation

    /// <summary>
    /// Verifies that CalculateResultAsync respects cancellation token.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_CancellationToken_Respected()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var duelId = 1;
        var winningOptionId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.CalculateResultAsync(duelId, winningOptionId, cts.Token));
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync respects cancellation token.
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_CancellationToken_Respected()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var duelId = 1;

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.CalculateWinningOptionIdAsync(duelId, DuelType.OpinionMatch, cts.Token));
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
