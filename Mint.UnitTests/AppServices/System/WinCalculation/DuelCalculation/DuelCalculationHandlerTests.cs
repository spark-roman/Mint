using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

        // Assert
        var losingResults = result.VoteResults.Where(v => v.PayoutInstruction == null).ToList();
        Assert.Single(losingResults);
        Assert.Equal(4, losingResults.First().VoteAccountId);
    }

    /// <summary>
    /// Verifies that payout instructions use the system account as debit account.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_PayoutInstructionsUseSystemAccountAsDebit()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 4,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Один голос",
            Description = "Только один голос",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 4, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-1) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

        // Assert
        var winningPayouts = result.VoteResults.Where(v => v.PayoutInstruction != null).ToList();
        Assert.Equal(2, winningPayouts.Count);
        Assert.All(winningPayouts, p => Assert.True(p.PayoutInstruction!.Amount > 0));
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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, winningOptionId, votes, CancellationToken.None);

        // Assert
        var alicePayout = result.VoteResults.First(v => v.VoteAccountId == 2).PayoutInstruction!.Amount;
        Assert.True(alicePayout > 500m); // Alice bet 500, should receive more

        var bobPayout = result.VoteResults.First(v => v.VoteAccountId == 3).PayoutInstruction!.Amount;
        Assert.True(bobPayout > 100m); // Bob bet 100, should receive more
    }

    /// <summary>
    /// Verifies that CalculateResultAsync returns an empty result without payout instructions when there are no votes.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_NoVotes_ReturnsEmptyResult()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>([]);

        // Act
        var result = await handler.CalculateResultAsync(duel, 1, votes, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.DuelId);
        Assert.Equal(1, result.WinningOptionId);
        Assert.Equal(0m, result.TotalPot);
        Assert.Equal(0m, result.HouseCut);
        Assert.Empty(result.VoteResults);
    }

    /// <summary>
    /// Verifies that CalculateResultAsync refunds all bets when the duel ends in a draw.
    /// winningOptionId is null -> every voter receives a refund of the full bet amount.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_Draw_RefundsAllBets()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateResultAsync(duel, null, votes, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.WinningOptionId);
        Assert.Equal(900m, result.TotalPot); // 500 + 100 + 300
        Assert.Equal(0m, result.HouseCut); // no commission on refunds
        Assert.Equal(3, result.VoteResults.Count);

        // Every voter receives a refund instruction of the full bet amount
        Assert.All(result.VoteResults, v => Assert.NotNull(v.PayoutInstruction));
        Assert.All(result.VoteResults, v => Assert.Equal(BonusType.Refund, v.PayoutInstruction!.BonusType));
        Assert.All(result.VoteResults, v => Assert.Equal(1, v.PayoutInstruction!.DebitAccountId)); // system account

        Assert.Equal(500m, result.VoteResults.First(v => v.VoteAccountId == 2).PayoutInstruction!.Amount);
        Assert.Equal(100m, result.VoteResults.First(v => v.VoteAccountId == 3).PayoutInstruction!.Amount);
        Assert.Equal(300m, result.VoteResults.First(v => v.VoteAccountId == 4).PayoutInstruction!.Amount);
    }

    /// <summary>
    /// Verifies that CalculateResultAsync throws ArgumentNullException when duel is null.
    /// </summary>
    [Fact]
    public async Task CalculateResultAsync_NullDuel_ThrowsArgumentNullException()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => handler.CalculateResultAsync(null!, 1, votes, CancellationToken.None));
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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var now = DateTimeOffset.UtcNow;
        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(DuelType.OpinionMatch, votes, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns the correct winning option for duel 4.
    /// Duel 4: single vote on option 1 -> winner = [1].
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_SingleVote_ReturnsCorrectOption()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var now = DateTimeOffset.UtcNow;
        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 4, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-1) }
        ]);

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(DuelType.OpinionMatch, votes, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var now = DateTimeOffset.UtcNow;
        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 5, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-3) },
            new VoteDto { AccountId = 3, DuelId = 5, ChosenOptionId = 1, BetAmount = 200m, CreatedAt = now.AddHours(-3) },
            new VoteDto { AccountId = 4, DuelId = 5, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-3) }
        ]);

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(DuelType.OpinionMatch, votes, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public async Task CalculateWinningOptionIdAsync_MultipleEqualVotes_Returns2WinningOptions()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 6, ChosenOptionId = 11, BetAmount = 500m, CreatedAt = now.AddHours(-3) },
            new VoteDto { AccountId = 3, DuelId = 6, ChosenOptionId = 11, BetAmount = 200m, CreatedAt = now.AddHours(-3) },
            new VoteDto { AccountId = 1, DuelId = 6, ChosenOptionId = 12, BetAmount = 300m, CreatedAt = now.AddHours(-3) },
            new VoteDto { AccountId = 4, DuelId = 6, ChosenOptionId = 12, BetAmount = 300m, CreatedAt = now.AddHours(-3) }
        ]);

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(DuelType.OpinionMatch, votes, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns empty for duel with no votes.
    /// Duel 2 has no votes.
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_NoVotes_ReturnsEmptyList()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var votes = new ReadOnlyCollection<VoteDto>([]);

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(DuelType.OpinionMatch, votes, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync returns null when no rule matches the duel type.
    /// There is no rule registered for FactPrediction in the test setup.
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_NoMatchingRule_ReturnsNull()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);

        var now = DateTimeOffset.UtcNow;
        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act
        var result = await handler.CalculateWinningOptionIdAsync(DuelType.FactPrediction, votes, CancellationToken.None);

        // Assert
        Assert.Null(result);
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
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var winningOptionId = 1;
        var now = DateTimeOffset.UtcNow;

        var duel = new DuelDto
        {
            Id = 1,
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Bitcoin достигнет $100k?",
            Description = "Достигнет ли Bitcoin цены 100 тысяч долларов?",
            ExpiresAt = now.AddHours(48),
            Status = DuelStatus.Active,
            Options =
            [
                new DuelOptionDto { Id = 1, OptionCode =  "A", OptionText = "A" },
                new DuelOptionDto { Id = 2, OptionCode =  "B", OptionText = "B" }
            ]
        };

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.CalculateResultAsync(duel, winningOptionId, votes, cts.Token));
    }

    /// <summary>
    /// Verifies that CalculateWinningOptionIdAsync respects cancellation token.
    /// </summary>
    [Fact]
    public async Task CalculateWinningOptionIdAsync_CancellationToken_Respected()
    {
        // Arrange
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _fixture.GetHandler(_currentScope);
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        var now = DateTimeOffset.UtcNow;

        var votes = new ReadOnlyCollection<VoteDto>(
        [
            new VoteDto { AccountId = 2, DuelId = 1, ChosenOptionId = 1, BetAmount = 500m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 3, DuelId = 1, ChosenOptionId = 1, BetAmount = 100m, CreatedAt = now.AddHours(-2) },
            new VoteDto { AccountId = 4, DuelId = 1, ChosenOptionId = 2, BetAmount = 300m, CreatedAt = now.AddHours(-2) }
        ]);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => handler.CalculateWinningOptionIdAsync(DuelType.OpinionMatch, votes, cts.Token));
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
