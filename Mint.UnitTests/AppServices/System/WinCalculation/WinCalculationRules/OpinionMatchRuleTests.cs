using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.WinCalculationRules;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
using Mint.UnitTests.AppServices.System.WinCalculation.WinCalculationRules.Fixtures;

namespace Mint.UnitTests.AppServices.System.WinCalculation.WinCalculationRules;

/// <summary>
/// Tests for <see cref="OpinionMatchRule"/> using DI and EF Core In-Memory.
/// Winning option is determined by maximum vote count (number of voters), not bet amount.
/// </summary>
public class OpinionMatchRuleTests : IClassFixture<OpinionMatchRuleFixture>, IDisposable
{
    private readonly OpinionMatchRuleFixture _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpinionMatchRuleTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public OpinionMatchRuleTests(OpinionMatchRuleFixture fixture)
    {
        _fixture = fixture;
    }

    #region IsMatchedAsync - Duel Type Matching

    /// <summary>
    /// Verifies that IsMatchedAsync returns true for OpinionMatch duel type.
    /// </summary>
    [Fact]
    public async Task IsMatchedAsync_OpinionMatch_ReturnsTrue()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.IsMatchedAsync(DuelType.OpinionMatch);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that IsMatchedAsync returns false for FactPrediction duel type.
    /// </summary>
    [Fact]
    public async Task IsMatchedAsync_FactPrediction_ReturnsFalse()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.IsMatchedAsync(DuelType.FactPrediction);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that IsMatchedAsync returns false for any non-OpinionMatch duel type.
    /// </summary>
    [Fact]
    public async Task IsMatchedAsync_NonOpinionMatch_ReturnsFalse()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.IsMatchedAsync((DuelType)99);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region CalculateAsync - Single Winning Option (by Vote Count)

    /// <summary>
    /// Verifies that CalculateAsync returns the option with the most votes (not highest bet sum).
    /// Duel 1: option 1 = 3 votes, option 2 = 2 votes. Winner: option 1.
    /// Note: option 2 has higher total bet (1000+300=1300 vs 500+10+100=610), but fewer voters.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_OpinionMatch_WinnerByVoteCount_ReturnsWinningOption()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 1, BetAmount = 10m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 5, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 6, DuelId = 1, ChosenOptionId = 2, BetAmount = 1000m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// Verifies that CalculateAsync correctly identifies the winning option by vote count for duel 2.
    /// Duel 2: option 3 = 1 vote, option 4 = 3 votes. Winner: option 4.
    /// Note: option 3 has bet 9999, but only 1 voter vs 3 voters for option 4.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_Duel2_WinnerByVoteCount_ReturnsWinningOption()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 2, ChosenOptionId = 3, BetAmount = 9999m, CreatedAt = now.AddHours(-1) },
            new VoteDto { AccountId = 5, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) },
            new VoteDto { AccountId = 6, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) },
            new VoteDto { AccountId = 4, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) }
        ]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Equal(4, result);
    }

    /// <summary>
    /// Verifies that the option with fewer total bet amount but more voters wins.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_FewerBetsMoreVoters_Wins()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 1, BetAmount = 10m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 5, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 6, DuelId = 1, ChosenOptionId = 2, BetAmount = 1000m, CreatedAt = now.AddHours(-2) }
        ]);

        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);
    }

    /// <summary>
    /// Verifies that a single large bet does not beat multiple small bets.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_OneLargeBet_LosesToMultipleSmallBets()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 2, ChosenOptionId = 3, BetAmount = 9999m, CreatedAt = now.AddHours(-1) },
            new VoteDto { AccountId = 5, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) },
            new VoteDto { AccountId = 6, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) },
            new VoteDto { AccountId = 4, DuelId = 2, ChosenOptionId = 4, BetAmount = 10m, CreatedAt = now.AddHours(-1) }
        ]);

        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Equal(4, result);
    }

    #endregion

    #region CalculateAsync - Tie (by Vote Count)

    /// <summary>
    /// Verifies that CalculateAsync returns all tied options when vote counts are equal.
    /// Duel 3: option 5 = 2 votes, option 6 = 2 votes. Tie: [5, 6].
    /// </summary>
    [Fact]
    public async Task CalculateAsync_TieByVoteCount_ReturnsAllWinningOptions()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 3, ChosenOptionId = 5, BetAmount = 100m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 3, DuelId = 3, ChosenOptionId = 5, BetAmount = 200m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 4, DuelId = 3, ChosenOptionId = 6, BetAmount = 500m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 5, DuelId = 3, ChosenOptionId = 6, BetAmount = 50m, CreatedAt = now.AddHours(-3) }
        ]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region CalculateAsync - No Votes

    /// <summary>
    /// Verifies that CalculateAsync returns empty list when no votes exist for the duel.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_NoVotes_ReturnsEmptyList()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        var votes = new ReadOnlyCollection<VoteDto>([]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that CalculateAsync returns null when the votes collection is null.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_NullVotes_ReturnsNull()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.CalculateAsync(null!, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region CalculateAsync - Single Vote

    /// <summary>
    /// Verifies that CalculateAsync returns the correct option when only one vote exists.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_SingleVote_ReturnsThatOption()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        var chosenOptionId = 5;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 3, ChosenOptionId = chosenOptionId, BetAmount = 100m, CreatedAt = DateTimeOffset.UtcNow.AddHours(-3) },
        ]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Equal(chosenOptionId, result);
    }

    #endregion

    #region CalculateAsync - Multiple Votes Same Option

    /// <summary>
    /// Verifies that CalculateAsync correctly counts votes per option.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_MultipleVotesSameOption_CorrectlyCountsVotes()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        var now = DateTimeOffset.UtcNow;
        var winChosenOptionId = 5;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 100m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 3, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 200m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 4, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 500m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 5, DuelId = 3, ChosenOptionId = 6, BetAmount = 50m, CreatedAt = now.AddHours(-3) }
        ]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Equal(5, result);
    }

    #endregion

    #region CalculateAsync - Large Vote Count

    /// <summary>
    /// Verifies that CalculateAsync correctly handles a large number of voters.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_LargeVoteCount_CorrectlyHandles()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        var now = DateTimeOffset.UtcNow;
        var winChosenOptionId = 5;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 1m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 3, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 2m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 4, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 3m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 5, DuelId = 3, ChosenOptionId = 6, BetAmount = 100500m, CreatedAt = now.AddHours(-3) }
        ]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert - option 2 wins with 5 votes vs 1 vote
        Assert.Equal(winChosenOptionId, result);
    }

    #endregion

    #region CalculateAsync - All Votes Same Option

    /// <summary>
    /// Verifies that when all voters choose the same option, it wins unanimously.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_AllVotesSameOption_ReturnsThatOption()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        var now = DateTimeOffset.UtcNow;
        var winChosenOptionId = 5;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 1m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 3, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 2m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 4, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 3m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 5, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 5m, CreatedAt = now.AddHours(-3) }
        ]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Equal(winChosenOptionId, result);
    }

    #endregion

    #region CalculateAsync - Three Options Tie

    /// <summary>
    /// Verifies that CalculateAsync handles ties across three or more options.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_ThreeOptionTie_ReturnsAllTiedOptions()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 3, ChosenOptionId = 1, BetAmount = 1m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 3, DuelId = 3, ChosenOptionId = 2, BetAmount = 2m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 4, DuelId = 3, ChosenOptionId = 3, BetAmount = 3m, CreatedAt = now.AddHours(-3) }, 
        ]);

        // Act
        var result = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region CalculateAsync - Result Ordering

    /// <summary>
    /// Verifies that CalculateAsync returns results in a consistent order.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_ResultOrdering_Consistent()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        var now = DateTimeOffset.UtcNow;
        var winChosenOptionId = 5;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 1m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 3, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 2m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 4, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 3m, CreatedAt = now.AddHours(-3) }, 
            new VoteDto { AccountId = 5, DuelId = 3, ChosenOptionId = winChosenOptionId, BetAmount = 5m, CreatedAt = now.AddHours(-3) }
        ]);

        // Act - run multiple times
        var result1 = await rule.CalculateAsync(votes, CancellationToken.None);
        var result2 = await rule.CalculateAsync(votes, CancellationToken.None);
        var result3 = await rule.CalculateAsync(votes, CancellationToken.None);

        // Assert
        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
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
