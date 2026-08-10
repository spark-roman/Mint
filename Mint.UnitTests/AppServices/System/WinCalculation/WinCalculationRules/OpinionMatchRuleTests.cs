using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.WinCalculationRules;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
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
        await _fixture.ResetAsync();
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
        await _fixture.ResetAsync();
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
        await _fixture.ResetAsync();
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.CalculateAsync(1, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(1, result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.CalculateAsync(2, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(4, result);
    }

    /// <summary>
    /// Verifies that the option with fewer total bet amount but more voters wins.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_FewerBetsMoreVoters_Wins()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Option 1: 3 voters, total bet = 610
        // Option 2: 2 voters, total bet = 1300
        // Winner should be option 1 (more voters)
        var result = await rule.CalculateAsync(1, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(1, result);
    }

    /// <summary>
    /// Verifies that a single large bet does not beat multiple small bets.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_OneLargeBet_LosesToMultipleSmallBets()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Option 3: 1 voter with bet 9999
        // Option 4: 3 voters with bets 10 each
        // Winner should be option 4 (more voters)
        var result = await rule.CalculateAsync(2, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(4, result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.CalculateAsync(3, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(5, result);
        Assert.Contains(6, result);
    }

    /// <summary>
    /// Verifies that CalculateAsync returns tied options sorted correctly.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_Tie_ReturnsSortedOptions()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.CalculateAsync(3, CancellationToken.None);

        // Assert
        var sorted = result.OrderBy(x => x).ToList();
        Assert.Equal(sorted, result);
    }

    /// <summary>
    /// Verifies that ties are determined by vote count, not bet amount.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_TieByVoteCount_BetsDiffer()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Option 5: 2 voters, bets 100+200 = 300
        // Option 6: 2 voters, bets 500+50 = 550
        // Same vote count -> tie
        var result = await rule.CalculateAsync(3, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(5, result);
        Assert.Contains(6, result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Create a duel with no votes
        using var scope = _fixture.ServiceProvider.CreateScope();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();

        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Test duel",
            Description = "Test description",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = new List<DuelOptionCreateDto>
            {
                new() { OptionText = "Yes", OptionCode = "yes" },
                new() { OptionText = "No", OptionCode = "no" }
            }
        }, CancellationToken.None);

        // Act
        var result = await rule.CalculateAsync(duelId, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that CalculateAsync returns empty list for non-existent duel.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_NonExistentDuel_ReturnsEmptyList()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act
        var result = await rule.CalculateAsync(999, CancellationToken.None);

        // Assert
        Assert.Empty(result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Create a duel with a single vote
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Test duel",
            Description = "Test description",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = new List<DuelOptionCreateDto>
            {
                new() { OptionText = "Yes", OptionCode = "yes" },
                new() { OptionText = "No", OptionCode = "no" }
            }
        }, CancellationToken.None);

        var vote = new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            ChosenOptionId = 1,
            BetAmount = 500m
        };

        await voteRepository.CreateVoteAsync(vote, CancellationToken.None);

        // Act
        var result = await rule.CalculateAsync(duelId, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(1, result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Create a duel with multiple votes on the same option
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Test duel",
            Description = "Test description",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = new List<DuelOptionCreateDto>
            {
                new() { OptionText = "Yes", OptionCode = "yes" },
                new() { OptionText = "No", OptionCode = "no" }
            }
        }, CancellationToken.None);

        // 3 votes on option 1, 1 vote on option 2
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            ChosenOptionId = 1,
            BetAmount = 100m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 3,
            ChosenOptionId = 1,
            BetAmount = 200m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 4,
            ChosenOptionId = 1,
            BetAmount = 300m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 5,
            ChosenOptionId = 2,
            BetAmount = 150m
        }, CancellationToken.None);

        // Act
        var result = await rule.CalculateAsync(duelId, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(1, result);
    }

    #endregion

    #region CalculateAsync - Zero Bet Amount

    /// <summary>
    /// Verifies that CalculateAsync correctly handles votes with zero bet amount.
    /// Each vote still counts as one voter regardless of bet amount.
    /// </summary>
    [Fact]
    public async Task CalculateAsync_ZeroBetAmount_VoteStillCounts()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Test duel",
            Description = "Test description",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = new List<DuelOptionCreateDto>
            {
                new() { OptionText = "Yes", OptionCode = "yes" },
                new() { OptionText = "No", OptionCode = "no" }
            }
        }, CancellationToken.None);

        // Option 1: 2 votes (one with 0 bet, one with 100)
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            ChosenOptionId = 1,
            BetAmount = 0m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 3,
            ChosenOptionId = 1,
            BetAmount = 100m
        }, CancellationToken.None);

        // Option 2: 1 vote
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 4,
            ChosenOptionId = 2,
            BetAmount = 1000m
        }, CancellationToken.None);

        // Act
        var result = await rule.CalculateAsync(duelId, CancellationToken.None);

        // Assert - option 1 wins with 2 votes vs 1 vote
        Assert.Single(result);
        Assert.Contains(1, result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Test duel",
            Description = "Test description",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = new List<DuelOptionCreateDto>
            {
                new() { OptionText = "Yes", OptionCode = "yes" },
                new() { OptionText = "No", OptionCode = "no" }
            }
        }, CancellationToken.None);

        // Option 1: 1 vote with huge bet
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            ChosenOptionId = 1,
            BetAmount = 999999.99m
        }, CancellationToken.None);

        // Option 2: 5 votes with small bets
        for (int i = 0; i < 5; i++)
        {
            await voteRepository.CreateVoteAsync(new VoteCreateDto
            {
                DuelId = duelId,
                AccountId = 3 + i,
                ChosenOptionId = 2,
                BetAmount = 1m
            }, CancellationToken.None);
        }

        // Act
        var result = await rule.CalculateAsync(duelId, CancellationToken.None);

        // Assert - option 2 wins with 5 votes vs 1 vote
        Assert.Single(result);
        Assert.Contains(2, result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Test duel",
            Description = "Test description",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = new List<DuelOptionCreateDto>
            {
                new() { OptionText = "Yes", OptionCode = "yes" },
                new() { OptionText = "No", OptionCode = "no" }
            }
        }, CancellationToken.None);

        // All 5 voters choose option 1
        for (int i = 0; i < 5; i++)
        {
            await voteRepository.CreateVoteAsync(new VoteCreateDto
            {
                DuelId = duelId,
                AccountId = 2 + i,
                ChosenOptionId = 1,
                BetAmount = (i + 1) * 10m
            }, CancellationToken.None);
        }

        // Act
        var result = await rule.CalculateAsync(duelId, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Contains(1, result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();

        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Test duel",
            Description = "Test description",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = new List<DuelOptionCreateDto>
            {
                new() { OptionText = "A", OptionCode = "a" },
                new() { OptionText = "B", OptionCode = "b" },
                new() { OptionText = "C", OptionCode = "c" }
            }
        }, CancellationToken.None);

        // 1 vote each on 3 options -> tie
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            ChosenOptionId = 1,
            BetAmount = 100m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 3,
            ChosenOptionId = 2,
            BetAmount = 200m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 4,
            ChosenOptionId = 3,
            BetAmount = 300m
        }, CancellationToken.None);

        // Act
        var result = await rule.CalculateAsync(duelId, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Contains(1, result);
        Assert.Contains(2, result);
        Assert.Contains(3, result);
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
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var rule = _currentScope.ServiceProvider.GetRequiredService<OpinionMatchRule>();

        // Act - run multiple times
        var result1 = await rule.CalculateAsync(1, CancellationToken.None);
        var result2 = await rule.CalculateAsync(1, CancellationToken.None);
        var result3 = await rule.CalculateAsync(1, CancellationToken.None);

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
