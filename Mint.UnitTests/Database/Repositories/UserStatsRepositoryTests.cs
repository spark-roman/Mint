using Mint.Database.Entities.UserInteractive.Stats;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Users;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database;
using Mint.Database.Entities.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes;
using Mint.Database.Entities.System.Payouts;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="UserStatsRepository"/>
/// </summary>
public class UserStatsRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public UserStatsRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that creating user stats returns a valid stats ID.
    /// </summary>
    [Fact]
    public async Task CreateStatsAsync_CreatedStats_ReturnsStatsId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var stats = new UserStatsCreateDto
        {
            ExternalUserId = 1001,
            RankPoints = 100,
            TotalWins = 5,
            TotalLosses = 2
        };

        // Act
        var statsId = await repository.CreateStatsAsync(stats, CancellationToken.None);

        // Assert
        Assert.True(statsId > 0);
    }

    /// <summary>
    /// Verifies that creating user stats with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateStatsAsync_NullStats_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateStatsAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that retrieving user stats by user ID returns a valid UserStatsDto.
    /// </summary>
    [Fact]
    public async Task GetStatsByUserIdAsync_ExistingStats_ReturnsStatsDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var stats = new UserStatsCreateDto
        {
            ExternalUserId = 1001,
            RankPoints = 150,
            TotalWins = 10,
            TotalLosses = 3
        };

        // Act
        var statsId = await repository.CreateStatsAsync(stats, CancellationToken.None);
        var result = await repository.GetStatsByUserIdAsync(stats.ExternalUserId, (byte)AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statsId, result.Id);
        Assert.Equal(1, result.UserId);
        Assert.Equal(150, result.RankPoints);
        Assert.Equal(10, result.TotalWins);
        Assert.Equal(3, result.TotalLosses);
        Assert.NotEqual(default, result.UpdatedAt);
    }

    /// <summary>
    /// Verifies that retrieving non-existent user stats returns null.
    /// </summary>
    [Fact]
    public async Task GetStatsByUserIdAsync_NonExistentStats_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        var result = await repository.GetStatsByUserIdAsync(999, (byte)AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that updating user stats returns true and updates the data.
    /// </summary>
    [Fact]
    public async Task UpdateStatsAsync_ExistingStats_UpdatesSuccessfully()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var createStats = new UserStatsCreateDto
        {
            ExternalUserId = 1001,
            RankPoints = 100,
            TotalWins = 5,
            TotalLosses = 2
        };

        await repository.CreateStatsAsync(createStats, CancellationToken.None);

        var updateStats = new UserStatsUpdateDto
        {
            RankPoints = 200,
            TotalWins = 8,
            TotalLosses = 3
        };

        // Act
        var result = await repository.UpdateStatsAsync(createStats.ExternalUserId, (byte)AuthSystem.Tg, updateStats, CancellationToken.None);
        var updated = await repository.GetStatsByUserIdAsync(createStats.ExternalUserId,(byte)AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.NotNull(updated);
        Assert.Equal(200, updated.RankPoints);
        Assert.Equal(8, updated.TotalWins);
        Assert.Equal(3, updated.TotalLosses);
    }

    /// <summary>
    /// Verifies that updating non-existent user stats returns false.
    /// </summary>
    [Fact]
    public async Task UpdateStatsAsync_NonExistentStats_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var updateStats = new UserStatsUpdateDto
        {
            RankPoints = 100,
            TotalWins = 5,
            TotalLosses = 2
        };

        // Act
        var result = await repository.UpdateStatsAsync(999, (byte)AuthSystem.Tg, updateStats, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that updating user stats with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task UpdateStatsAsync_NullStats_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.UpdateStatsAsync(1, (byte)AuthSystem.Tg, null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that creating stats for a different user returns a different ID.
    /// </summary>
    [Fact]
    public async Task CreateStatsAsync_DifferentUsers_ReturnsDifferentIds()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var stats1 = new UserStatsCreateDto
        {
            ExternalUserId = 1001,
            RankPoints = 50,
            TotalWins = 2,
            TotalLosses = 1
        };

        var stats2 = new UserStatsCreateDto
        {
            ExternalUserId = 1002,
            RankPoints = 75,
            TotalWins = 3,
            TotalLosses = 1
        };

        // Act
        var statsId1 = await repository.CreateStatsAsync(stats1, CancellationToken.None);
        var statsId2 = await repository.CreateStatsAsync(stats2, CancellationToken.None);

        // Assert
        Assert.NotEqual(statsId1, statsId2);
        Assert.True(statsId1 > 0);
        Assert.True(statsId2 > 0);
    }

    /// <summary>
    /// Verifies that UpdateStatsAsync updates only the stats of the user
    /// with the matching system type when several users share the same external user id.
    /// </summary>
    [Fact]
    public async Task UpdateStatsAsync_SameExternalUserIdDifferentSystemType_UpdatesOnlyMatchingUser()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        // User 1001 (SystemType = Tg) with stats
        var createStats = new UserStatsCreateDto
        {
            ExternalUserId = 1001,
            RankPoints = 100,
            TotalWins = 5,
            TotalLosses = 2
        };
        await repository.CreateStatsAsync(createStats, CancellationToken.None);

        // Another user with the same external user id but a different system type
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        context.Users.Add(new UserEntity
        {
            Id = 10,
            ExternalUserId = 1001,
            SystemType = 2,
            FirstName = "Web",
            LastName = "Alice",
            UserName = "web_alice",
            CreatedAt = DateTimeOffset.UtcNow,
            Status = 1
        });
        await context.SaveChangesAsync(CancellationToken.None);

        // Directly insert stats for the web user (SystemType = 2) since CreateStatsAsync
        // uses the default system type (Tg) and would create stats for the wrong user
        context.UserStats.Add(new UserStatsEntity
        {
            UserId = 10,
            RankPoints = 10,
            TotalWins = 1,
            TotalLosses = 0
        });
        await context.SaveChangesAsync(CancellationToken.None);

        var updateStats = new UserStatsUpdateDto
        {
            RankPoints = 999,
            TotalWins = 9,
            TotalLosses = 9
        };

        // Act - update the stats of the web user (SystemType = 2)
        var result = await repository.UpdateStatsAsync(1001, 2, updateStats, CancellationToken.None);

        // Assert - the web user stats were updated
        Assert.True(result);
        var webStatsAfter = await repository.GetStatsByUserIdAsync(1001, 2, CancellationToken.None);
        Assert.NotNull(webStatsAfter);
        Assert.Equal(999, webStatsAfter.RankPoints);
        Assert.Equal(9, webStatsAfter.TotalWins);
        Assert.Equal(9, webStatsAfter.TotalLosses);

        // Assert - the Telegram user stats were not affected
        var tgStatsAfter = await repository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(tgStatsAfter);
        Assert.Equal(100, tgStatsAfter.RankPoints);
        Assert.Equal(5, tgStatsAfter.TotalWins);
        Assert.Equal(2, tgStatsAfter.TotalLosses);
    }

    /// <summary>
    /// Verifies that UpdateStatsAsync returns false when no user with the given
    /// external user id and system type exists.
    /// </summary>
    [Fact]
    public async Task UpdateStatsAsync_NonExistentSystemType_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var updateStats = new UserStatsUpdateDto
        {
            RankPoints = 100,
            TotalWins = 5,
            TotalLosses = 2
        };

        // Act - user 1001 exists only with SystemType = 1
        var result = await repository.UpdateStatsAsync(1001, 2, updateStats, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that UpdateStatsByAccountIdAsync updates the stats of the user owning the account.
    /// </summary>
    [Fact]
    public async Task UpdateStatsByAccountIdAsync_ExistingAccount_UpdatesStats()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var createStats = new UserStatsCreateDto
        {
            ExternalUserId = 1001,
            RankPoints = 100,
            TotalWins = 5,
            TotalLosses = 2
        };
        await repository.CreateStatsAsync(createStats, CancellationToken.None);

        var updateStats = new UserStatsUpdateDto
        {
            RankPoints = 200,
            TotalWins = 8,
            TotalLosses = 3,
            TotalDraws = 1
        };

        // Act - account 1 belongs to user 1001
        var result = await repository.UpdateStatsByAccountIdAsync(1, updateStats, CancellationToken.None);

        // Assert
        Assert.True(result);
        var updated = await repository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(updated);
        Assert.Equal(200, updated.RankPoints);
        Assert.Equal(8, updated.TotalWins);
        Assert.Equal(3, updated.TotalLosses);
        Assert.Equal(1, updated.TotalDraws);
    }

    /// <summary>
    /// Verifies that UpdateStatsByAccountIdAsync returns false for a non-existent account.
    /// </summary>
    [Fact]
    public async Task UpdateStatsByAccountIdAsync_NonExistentAccount_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        var updateStats = new UserStatsUpdateDto
        {
            RankPoints = 100,
            TotalWins = 5,
            TotalLosses = 2
        };

        // Act
        var result = await repository.UpdateStatsByAccountIdAsync(999, updateStats, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    #region GetDuelHistoryAsync

    /// <summary>
    /// Verifies that GetDuelHistoryAsync returns an empty list when the account has no votes.
    /// </summary>
    [Fact]
    public async Task GetDuelHistoryAsync_NoVotes_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        // Act
        var result = await repository.GetDuelHistoryAsync(accountId: 1, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that GetDuelHistoryAsync returns an empty list when the account has only open duels.
    /// </summary>
    [Fact]
    public async Task GetDuelHistoryAsync_OpenDuels_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var category = await context.UserCategories.FirstAsync();
        var duel = new DuelEntity
        {
            CategoryId = category.Id,
            Category = category,
            DuelType = DuelType.OpinionMatch,
            Question = "Is TON going up?",
            Description = "Will TON go up?",
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1),
            Status = DuelStatus.Active
        };
        context.Duels.Add(duel);
        await context.SaveChangesAsync(CancellationToken.None);

        var vote = new VoteEntity
        {
            AccountId = 1,
            Account = context.Accounts.First(a => a.Id == 1),
            DuelId = duel.Id,
            Duel = duel,
            ChosenOptionId = 0,
            BetAmount = 100m,
            TransactionId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };
        context.Votes.Add(vote);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetDuelHistoryAsync(accountId: 1, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that GetDuelHistoryAsync returns a history entry with Win result
    /// when the payout BonusType is Payout.
    /// </summary>
    [Fact]
    public async Task GetDuelHistoryAsync_WinResult_ReturnsWin()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var category = await context.UserCategories.FirstAsync();
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(-1);

        var duel = new DuelEntity
        {
            CategoryId = category.Id,
            Category = category,
            DuelType = DuelType.OpinionMatch,
            Question = "Is TON going up?",
            Description = "Will TON go up?",
            ExpiresAt = expiresAt,
            Status = DuelStatus.Closed
        };
        context.Duels.Add(duel);
        await context.SaveChangesAsync(CancellationToken.None);

        var vote = new VoteEntity
        {
            AccountId = 1,
            Account = context.Accounts.First(a => a.Id == 1),
            DuelId = duel.Id,
            Duel = duel,
            ChosenOptionId = 0,
            BetAmount = 100m,
            TransactionId = 1,
            CreatedAt = now.AddMinutes(-2)
        };
        context.Votes.Add(vote);
        await context.SaveChangesAsync(CancellationToken.None);

        var payout = new PayoutEntity
        {
            DuelId = duel.Id,
            Duel = duel,
            CreditAccountId = 1,
            DebitAccountId = 2,
            Amount = 200m,
            Status = 0,
            BonusType = BonusType.Payout,
            VoteId = vote.Id,
            Vote = vote,
            PlannedAt = now
        };
        context.Payouts.Add(payout);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetDuelHistoryAsync(accountId: 1, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(DuelVoteResultType.Win, result[0].Result);
        Assert.Equal(100m, result[0].BetAmount);
        Assert.Equal(200m, result[0].PayoutAmount);
    }

    /// <summary>
    /// Verifies that GetDuelHistoryAsync returns a history entry with Draw result
    /// when the payout BonusType is Refund.
    /// </summary>
    [Fact]
    public async Task GetDuelHistoryAsync_DrawResult_ReturnsDraw()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var category = await context.UserCategories.FirstAsync();
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(-1);

        var duel = new DuelEntity
        {
            CategoryId = category.Id,
            Category = category,
            DuelType = DuelType.OpinionMatch,
            Question = "Is BTC going up?",
            Description = "Will BTC go up?",
            ExpiresAt = expiresAt,
            Status = DuelStatus.Closed
        };
        context.Duels.Add(duel);
        await context.SaveChangesAsync(CancellationToken.None);

        var vote = new VoteEntity
        {
            AccountId = 1,
            Account = context.Accounts.First(a => a.Id == 1),
            DuelId = duel.Id,
            Duel = duel,
            ChosenOptionId = 0,
            BetAmount = 50m,
            TransactionId = 1,
            CreatedAt = now.AddMinutes(-2)
        };
        context.Votes.Add(vote);
        await context.SaveChangesAsync(CancellationToken.None);

        var payout = new PayoutEntity
        {
            DuelId = duel.Id,
            Duel = duel,
            CreditAccountId = 1,
            DebitAccountId = 2,
            Amount = 50m,
            Status = 0,
            BonusType = BonusType.Refund,
            VoteId = vote.Id,
            Vote = vote,
            PlannedAt = now
        };
        context.Payouts.Add(payout);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetDuelHistoryAsync(accountId: 1, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(DuelVoteResultType.Draw, result[0].Result);
        Assert.Equal(50m, result[0].BetAmount);
        Assert.Equal(50m, result[0].PayoutAmount);
    }

    /// <summary>
    /// Verifies that GetDuelHistoryAsync returns a history entry with Loss result
    /// when the payout BonusType is NoActivity.
    /// </summary>
    [Fact]
    public async Task GetDuelHistoryAsync_LossResult_ReturnsLoss()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var category = await context.UserCategories.FirstAsync();
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(-1);

        var duel = new DuelEntity
        {
            CategoryId = category.Id,
            Category = category,
            DuelType = DuelType.OpinionMatch,
            Question = "Is ETH going up?",
            Description = "Will ETH go up?",
            ExpiresAt = expiresAt,
            Status = DuelStatus.Closed
        };
        context.Duels.Add(duel);
        await context.SaveChangesAsync(CancellationToken.None);

        var vote = new VoteEntity
        {
            AccountId = 1,
            Account = context.Accounts.First(a => a.Id == 1),
            DuelId = duel.Id,
            Duel = duel,
            ChosenOptionId = 0,
            BetAmount = 75m,
            TransactionId = 1,
            CreatedAt = now.AddMinutes(-2)
        };
        context.Votes.Add(vote);
        await context.SaveChangesAsync(CancellationToken.None);

        var payout = new PayoutEntity
        {
            DuelId = duel.Id,
            Duel = duel,
            CreditAccountId = 1,
            DebitAccountId = 2,
            Amount = 0m,
            Status = 0,
            BonusType = BonusType.NoActivity,
            VoteId = vote.Id,
            Vote = vote,
            PlannedAt = now
        };
        context.Payouts.Add(payout);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetDuelHistoryAsync(accountId: 1, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal(DuelVoteResultType.Loss, result[0].Result);
        Assert.Equal(75m, result[0].BetAmount);
        Assert.Equal(0m, result[0].PayoutAmount);
    }

    /// <summary>
    /// Verifies that GetDuelHistoryAsync returns multiple entries when the account
    /// has votes in multiple closed duels with the same ExpiresAt.
    /// </summary>
    [Fact]
    public async Task GetDuelHistoryAsync_MultipleDuelsSameExpiresAt_ReturnsAll()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var category = await context.UserCategories.FirstAsync();
        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(-1);

        // Create two duels with the same ExpiresAt
        for (int i = 0; i < 2; i++)
        {
            var duel = new DuelEntity
            {
                CategoryId = category.Id,
                Category = category,
                DuelType = DuelType.OpinionMatch,
                Question = $"Duel question {i}?",
                Description = $"Description {i}",
                ExpiresAt = expiresAt,
                Status = DuelStatus.Closed
            };
            context.Duels.Add(duel);
        }
        await context.SaveChangesAsync(CancellationToken.None);

        var votes = new List<VoteEntity>();
        for (int i = 0; i < 2; i++)
        {
            var vote = new VoteEntity
            {
                AccountId = 1,
                Account = context.Accounts.First(a => a.Id == 1),
                DuelId = context.Duels.Skip(i).First().Id,
                Duel = context.Duels.Skip(i).First(),
                ChosenOptionId = 0,
                BetAmount = 100m,
                TransactionId = 1,
                CreatedAt = now.AddMinutes(-2 - i)
            };
            context.Votes.Add(vote);
            await context.SaveChangesAsync(CancellationToken.None);
            votes.Add(vote);
        }

        // Add payouts for both votes
        foreach (var vote in votes)
        {
            var payout = new PayoutEntity
            {
                DuelId = vote.DuelId,
                Duel = vote.Duel,
                CreditAccountId = 1,
                DebitAccountId = 2,
                Amount = 200m,
                Status = 0,
                BonusType = BonusType.Payout,
                VoteId = vote.Id,
                Vote = vote,
                PlannedAt = now
            };
            context.Payouts.Add(payout);
        }
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetDuelHistoryAsync(accountId: 1, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, r =>
        {
            Assert.Equal(DuelVoteResultType.Win, r.Result);
            Assert.Equal(100m, r.BetAmount);
            Assert.Equal(200m, r.PayoutAmount);
        });
    }

    /// <summary>
    /// Verifies that GetDuelHistoryAsync only returns duels with the latest ExpiresAt
    /// when the account has closed duels with different ExpiresAt values.
    /// </summary>
    [Fact]
    public async Task GetDuelHistoryAsync_DifferentExpiresAt_ReturnsOnlyLatest()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);

        var category = await context.UserCategories.FirstAsync();
        var now = DateTimeOffset.UtcNow;

        // Create an older closed duel
        var olderDuel = new DuelEntity
        {
            CategoryId = category.Id,
            Category = category,
            DuelType = DuelType.OpinionMatch,
            Question = "Old duel?",
            Description = "Old",
            ExpiresAt = now.AddMinutes(-3),
            Status = DuelStatus.Closed
        };
        context.Duels.Add(olderDuel);
        await context.SaveChangesAsync(CancellationToken.None);

        // Create a newer closed duel
        var newerDuel = new DuelEntity
        {
            CategoryId = category.Id,
            Category = category,
            DuelType = DuelType.OpinionMatch,
            Question = "New duel?",
            Description = "New",
            ExpiresAt = now.AddMinutes(-1),
            Status = DuelStatus.Closed
        };
        context.Duels.Add(newerDuel);
        await context.SaveChangesAsync(CancellationToken.None);

        // Vote for the older duel
        var olderVote = new VoteEntity
        {
            AccountId = 1,
            Account = context.Accounts.First(a => a.Id == 1),
            DuelId = olderDuel.Id,
            Duel = olderDuel,
            ChosenOptionId = 0,
            BetAmount = 100m,
            TransactionId = 1,
            CreatedAt = now.AddMinutes(-4)
        };
        context.Votes.Add(olderVote);
        await context.SaveChangesAsync(CancellationToken.None);

        // Vote for the newer duel
        var newerVote = new VoteEntity
        {
            AccountId = 1,
            Account = context.Accounts.First(a => a.Id == 1),
            DuelId = newerDuel.Id,
            Duel = newerDuel,
            ChosenOptionId = 0,
            BetAmount = 50m,
            TransactionId = 2,
            CreatedAt = now.AddMinutes(-2)
        };
        context.Votes.Add(newerVote);
        await context.SaveChangesAsync(CancellationToken.None);

        // Add payout for the newer vote only
        var payout = new PayoutEntity
        {
            DuelId = newerDuel.Id,
            Duel = newerDuel,
            CreditAccountId = 1,
            DebitAccountId = 2,
            Amount = 100m,
            Status = 0,
            BonusType = BonusType.Payout,
            VoteId = newerVote.Id,
            Vote = newerVote,
            PlannedAt = now
        };
        context.Payouts.Add(payout);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetDuelHistoryAsync(accountId: 1, CancellationToken.None);

        // Assert - only the newer duel should be returned
        Assert.Single(result);
        Assert.Equal(newerDuel.Id, result[0].DuelId);
        Assert.Equal(DuelVoteResultType.Win, result[0].Result);
    }

    #endregion
}
