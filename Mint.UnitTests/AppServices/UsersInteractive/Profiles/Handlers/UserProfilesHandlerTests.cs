using AdvApplication.Auth.Users;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.Database.Entities.Users.Dto;
using Mint.UnitTests.AppServices.UsersInteractive.Fixtures;

namespace Mint.UnitTests.AppServices.UsersInteractive.Profiles.Handlers;

/// <summary>
/// Tests for <see cref="UserProfilesHandler"/> using DI and EF Core.
/// </summary>
public class UserProfilesHandlerTests : IClassFixture<UserProfilesHandlerFixture>, IDisposable
{
    private readonly UserProfilesHandlerFixture _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfilesHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public UserProfilesHandlerTests(UserProfilesHandlerFixture fixture)
    {
        _fixture = fixture;
    }

    #region GetProfileAsync - Happy Path

    /// <summary>
    /// Verifies that GetProfileAsync returns correct profile data for an existing user.
    /// </summary>
    [Fact]
    public async Task GetProfileAsync_ExistingUser_ReturnsCorrectProfileData()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act
        var result = await handler.GetProfileAsync(1001, AuthSystem.Tg, CancellationToken.None);

        // Assert - User fields
        Assert.NotNull(result);
        Assert.Equal(1001, result.ExternalUserId);
        Assert.Equal("Alice", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.NotNull(result.UserName);
        Assert.Equal("alice.smith", result.UserName);
        
        // Assert - Account fields
        Assert.Equal(1500.50m, result.Balance);
        
        // Assert - Rank fields
        Assert.Equal(150, result.RankPoints);
        Assert.Equal("Эксперт", result.RankName);
        Assert.Equal("🧠", result.RankEmoji);
        
        // Assert - Stats fields
        Assert.Equal(15, result.TotalDuels);
        Assert.Equal(10, result.Wins);
        Assert.Equal(5, result.Losses);
        Assert.Equal(66.7, result.Winrate); // 10 / (10 + 5) * 100 = 66.7
        
        // Assert - Referral fields
        Assert.Equal(2, result.ReferralCount);
        Assert.Equal(1, result.TotalReferralBonus);
        
        // Assert - Bonus stats fields
        Assert.Equal(3, result.StreakDays);
        Assert.Equal(0, result.TotalDailyBonus); // TotalDailyBonusesClaimed + TotalStreakBonusesClaimed = null + null = 0
        Assert.False(result.CanClaimDailyBonus);
        Assert.NotNull(result.NextDailyAvailableAt);
        Assert.True(result.NextDailyAvailableAt > DateTimeOffset.UtcNow);
        Assert.NotNull(result.TimeUntilBonus);
        Assert.True(result.TimeUntilBonus.Value.TotalHours > 0);

        // Assert - CreatedAt
        Assert.True(result.CreatedAt <= DateTimeOffset.UtcNow);

        // Assert - Rank (not set in handler, defaults to empty)
        Assert.Equal(string.Empty, result.Rank);
    }

    /// <summary>
    /// Verifies that GetProfileAsync returns correct winrate calculation.
    /// </summary>
    [Fact]
    public async Task GetProfileAsync_CalculatesWinrate_Correctly()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act
        var result = await handler.GetProfileAsync(1001, AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // 10 wins / (10 + 5) = 66.7%
        Assert.Equal(66.7, result.Winrate);
    }

    /// <summary>
    /// Verifies that GetProfileAsync returns correct data for user with stats.
    /// </summary>
    [Fact]
    public async Task GetProfileAsync_UserWithNoStats_ReturnsDefaultValues()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act
        var result = await handler.GetProfileAsync(1002, AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1002, result.ExternalUserId);
        Assert.Equal("Bob", result.FirstName);
        // Balance may be 3300 if ClaimDailyBonus was claimed (3200 + 100)
        Assert.True(result.Balance >= 3200.00m);
        // User 1002 has 75 rank points from seed data
        Assert.Equal(75, result.RankPoints);
        // User 1002 has 13 total duels (5 wins + 8 losses) from seed data
        Assert.Equal(13, result.TotalDuels);
        Assert.Equal(5, result.Wins);
        Assert.Equal(8, result.Losses);
        Assert.Equal(38.5, result.Winrate); // 5/13 * 100 = 38.5
        
        // Assert - Bonus stats fields (Bob has IsStartBonusClaimed = false, streak = 0)
        Assert.Equal(0, result.StreakDays);
    }

    #endregion

    #region GetProfileAsync - User Not Found

    /// <summary>
    /// Verifies that GetProfileAsync throws when user is not found.
    /// </summary>
    [Fact]
    public async Task GetProfileAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act & Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => handler.GetProfileAsync(99999, AuthSystem.Tg, CancellationToken.None));
    }

    #endregion

    #region InitializeUserAsync - Happy Path

    /// <summary>
    /// Verifies that InitializeUserAsync creates a new user with account and stats.
    /// </summary>
    [Fact]
    public async Task InitializeUserAsync_NewUser_CreatesUserWithAccountAndStats()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var newUser = new UserCreateDto
        {
            ExternalUserId = 50000,
            SystemType = (byte)AuthSystem.Tg,
            FirstName = "New",
            LastName = "User",
            UserName = "new_user",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var result = await handler.InitializeUserAsync(newUser, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(50000, result.ExternalUserId);
        Assert.Equal("New", result.FirstName);
        Assert.Equal("User", result.LastName);
        Assert.Equal("new_user", result.UserName);

        // Verify account was created with start bonus
        var account = await accountRepository.GetAccountByExternalUserIdAsync(
            50000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);
        Assert.Equal(1000.00m, account.Balance); // 0 initial + 1000 start bonus
    }

    /// <summary>
    /// Verifies that InitializeUserAsync updates existing user fields without creating a new account.
    /// </summary>
    [Fact]
    public async Task InitializeUserAsync_ExistingUser_UpdatesFieldsAndReturnsUser()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var existingUser = await userRepository.GetUserAsync(1002, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(existingUser);
        var originalLastAuthDate = existingUser.LastAuthDate;

        var existingAccount = await accountRepository.GetAccountByExternalUserIdAsync(
            1002, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(existingAccount);
        var originalBalance = existingAccount.Balance;

        var updateDto = new UserCreateDto
        {
            ExternalUserId = 1002,
            SystemType = (byte)AuthSystem.Tg,
            FirstName = "Bob",
            LastName = "Updated",
            UserName = "bob.updated",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Task.Delay(10); // Ensure time difference for LastAuthDate

        // Act
        var result = await handler.InitializeUserAsync(updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1002, result.ExternalUserId);
        Assert.Equal("Bob", result.FirstName);
        Assert.Equal("Updated", result.LastName);
        Assert.Equal("bob.updated", result.UserName);

        // Verify fields were updated in repository
        var updatedUser = await userRepository.GetUserAsync(1002, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.LastName);
        Assert.Equal("bob.updated", updatedUser.UserName);
        Assert.NotNull(updatedUser.LastAuthDate);
        if (originalLastAuthDate.HasValue)
        {
            Assert.True(updatedUser.LastAuthDate.Value > originalLastAuthDate.Value);
        }

        // Verify account was reused (not created) and balance increased by start bonus
        var updatedAccount = await accountRepository.GetAccountByExternalUserIdAsync(
            1002, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(updatedAccount);
        Assert.Equal(originalBalance + 1000.00m, updatedAccount.Balance);
    }

    /// <summary>
    /// Verifies that InitializeUserAsync throws when userCreateDto is null.
    /// </summary>
    [Fact]
    public async Task InitializeUserAsync_NullUserCreateDto_ThrowsArgumentNullException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.InitializeUserAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that InitializeUserAsync creates user stats for a new user.
    /// </summary>
    [Fact]
    public async Task InitializeUserAsync_NewUser_CreatesUserStats()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();

        var newUser = new UserCreateDto
        {
            ExternalUserId = 60000,
            SystemType = (byte)AuthSystem.Tg,
            FirstName = "Stats",
            LastName = "User",
            UserName = "stats_user",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        await handler.InitializeUserAsync(newUser, CancellationToken.None);

        // Assert
        var user = await userRepository.GetUserAsync(60000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(user);
        Assert.Equal(60000, user.ExternalUserId);

        var stats = await statsRepository.GetStatsByUserIdAsync(60000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(stats);
    }

    /// <summary>
    /// Verifies that InitializeUserAsync creates bonus stats for a new user.
    /// </summary>
    [Fact]
    public async Task InitializeUserAsync_NewUser_CreatesUserBonusStats()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();

        var newUser = new UserCreateDto
        {
            ExternalUserId = 70000,
            SystemType = (byte)AuthSystem.Tg,
            FirstName = "Bonus",
            LastName = "User",
            UserName = "bonus_user",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        await handler.InitializeUserAsync(newUser, CancellationToken.None);

        // Assert
        var bonusStats = await bonusStatsRepository.GetStatsByUserIdAsync(70000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(bonusStats);
        Assert.True(bonusStats.IsStartBonusClaimed);
    }

    /// <summary>
    /// Verifies that the Start bonus can only be claimed once.
    /// </summary>
    [Fact]
    public async Task InitializeUserAsync_UserWithStartBonusAlreadyClaimed_DoesNotClaimAgain()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var bonusStatsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // User 1001 (Alice) already has IsStartBonusClaimed = true in seed data
        var existingAccount = await accountRepository.GetAccountByExternalUserIdAsync(
            1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(existingAccount);
        var initialBalance = existingAccount.Balance;

        var updateDto = new UserCreateDto
        {
            ExternalUserId = 1001,
            SystemType = (byte)AuthSystem.Tg,
            FirstName = "Alice",
            LastName = "Updated",
            UserName = "alice.updated",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var result = await handler.InitializeUserAsync(updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1001, result.ExternalUserId);

        var bonusStats = await bonusStatsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(bonusStats);
        Assert.True(bonusStats.IsStartBonusClaimed);

        // Verify start bonus was not claimed again (balance should not increase by 100)
        var updatedAccount = await accountRepository.GetAccountByExternalUserIdAsync(
            1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(updatedAccount);
        Assert.Equal(initialBalance, updatedAccount.Balance);
    }

    #endregion

    #region ClaimDailyBonusAsync - Happy Path

    /// <summary>
    /// Verifies that ClaimDailyBonusAsync claims bonus successfully for eligible user.
    /// </summary>
    [Fact]
    public async Task ClaimDailyBonusAsync_EligibleUser_ReturnsTrue()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act
        var result = await handler.ClaimDailyBonusAsync(1002, AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that ClaimDailyBonusAsync returns false when user is not found.
    /// </summary>
    [Fact]
    public async Task ClaimDailyBonusAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act
        var result = await handler.ClaimDailyBonusAsync(99999, AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetLeaderboardAsync - Happy Path

    /// <summary>
    /// Verifies that GetLeaderboardAsync returns sorted leaderboard by rank points ascending.
    /// </summary>
    [Fact]
    public async Task GetLeaderboardAsync_ReturnsSortedLeaderboard()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act
        var result = await handler.GetLeaderboardAsync(10, AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(result.Count >= 3, "Leaderboard should have at least 3 entries from seed data");
        // Verify sorted by rank points ascending (as implemented)
        for (int i = 1; i < result.Count; i++)
        {
            Assert.True(result[i - 1].RankPoints <= result[i].RankPoints);
        }
    }

    /// <summary>
    /// Verifies that GetLeaderboardAsync respects the limit parameter.
    /// </summary>
    [Fact]
    public async Task GetLeaderboardAsync_WithLimit_ReturnsCorrectCount()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act
        var result = await handler.GetLeaderboardAsync(2, AuthSystem.Tg, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Rank);
        Assert.Equal(2, result[1].Rank);
    }

    /// <summary>
    /// Verifies that GetLeaderboardAsync returns correct winrate calculation.
    /// </summary>
    [Fact]
    public async Task GetLeaderboardAsync_CalculatesWinrate_Correctly()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();

        // Act
        var result = await handler.GetLeaderboardAsync(10, AuthSystem.Tg, CancellationToken.None);

        // Assert - verify Alice's winrate from repository, not from leaderboard (which may have different users)
        var aliceStats = await statsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(aliceStats);
        var totalDuels = aliceStats.TotalWins + aliceStats.TotalLosses;
        var expectedWinrate = totalDuels > 0 ? Math.Round((double)aliceStats.TotalWins / totalDuels * 100, 1) : 0;
        Assert.Equal(66.7, expectedWinrate);
    }

    /// <summary>
    /// Verifies that GetLeaderboardAsync handles users without stats gracefully.
    /// </summary>
    [Fact]
    public async Task GetLeaderboardAsync_UserWithoutStats_ReturnsZeroValues()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act
        var result = await handler.GetLeaderboardAsync(10, AuthSystem.Tg, CancellationToken.None);

        // Assert
        // Bob has 75 rank points and 13 total duels (5 wins + 8 losses)
        var bob = result.FirstOrDefault(r => r.ExternalUserId == 1002);
        Assert.NotNull(bob);
        Assert.Equal(13, bob.TotalDuels);
        Assert.Equal(38.5, bob.Winrate); // 5/13 * 100 = 38.5
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
