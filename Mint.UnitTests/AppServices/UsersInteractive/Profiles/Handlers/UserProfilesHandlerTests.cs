using AdvApplication.Auth.Users;
using HashidsNet;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.Settings;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Accounts.Dto;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
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
        Assert.Equal(10, result.TotalWins);
        Assert.Equal(5, result.TotalLosses);
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
        Assert.Equal(5, result.TotalWins);
        Assert.Equal(8, result.TotalLosses);
        Assert.Equal(38.5, result.Winrate); // 5/13 * 100 = 38.5
        
        // Assert - Bonus stats fields (Bob has IsStartBonusClaimed = false)
        // StreakDays is order-dependent: ClaimDailyBonusAsync test may claim the daily bonus
        // for user 1002 first and increment the streak from 0 (seeded) to 1 in the shared fixture DB.
        Assert.InRange(result.StreakDays, 0, 1);
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
        Assert.Equal(5000.00m, account.Balance); // 0 initial + 1000 start bonus
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
        Assert.Equal(originalBalance + 5000.00m, updatedAccount.Balance);
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

    #region ProcessReferralAsync

    /// <summary>
    /// Encodes an external user id into a referral code using the same <see cref="IHashids"/> configuration as DI.
    /// </summary>
    /// <param name="externalUserId">External user id to encode.</param>
    /// <returns>Referral code.</returns>
    private string EncodeReferralCode(long externalUserId)
    {
        var hashids = _currentScope!.ServiceProvider.GetRequiredService<IHashids>();
        return hashids.EncodeLong(externalUserId);
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws ArgumentNullException when the referral code is null.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_NullReferralCode_ThrowsArgumentNullException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.ProcessReferralAsync(1002, AuthSystem.Tg, null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws ArgumentNullException when the referral code is empty.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_EmptyReferralCode_ThrowsArgumentNullException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.ProcessReferralAsync(1002, AuthSystem.Tg, string.Empty, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws ArgumentException when the referral code cannot be decoded.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_InvalidReferralCode_ThrowsArgumentException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentException>(() => handler.ProcessReferralAsync(1002, AuthSystem.Tg, "!!!invalid-code!!!", CancellationToken.None));
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws ArgumentException when a user tries to invite himself.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_SelfReferral_ThrowsArgumentException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var referralCode = EncodeReferralCode(1002);

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentException>(() => handler.ProcessReferralAsync(1002, AuthSystem.Tg, referralCode, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws InvalidOperationException when the referrer does not exist.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_ReferrerNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var referralCode = EncodeReferralCode(99999);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.ProcessReferralAsync(1002, AuthSystem.Tg, referralCode, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws InvalidOperationException when the new user does not exist.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_NewUserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var referralCode = EncodeReferralCode(1001);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.ProcessReferralAsync(99999, AuthSystem.Tg, referralCode, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync sets InvitedByUserId for the new user and increments the referrer's referral count.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_ValidReferral_SetsInvitedByAndIncrementsReferrerCount()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        var referralCode = EncodeReferralCode(1001);

        var newUserStatsBefore = await statsRepository.GetStatsByUserIdAsync(1002, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.Null(newUserStatsBefore!.InvitedByUserId);

        var referrerStatsBefore = await statsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);

        // Act
        await handler.ProcessReferralAsync(1002, AuthSystem.Tg, referralCode, CancellationToken.None);

        // Assert - new user is linked to the referrer (by internal user id)
        var newUserStatsAfter = await statsRepository.GetStatsByUserIdAsync(1002, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(newUserStatsAfter);
        Assert.Equal(1, newUserStatsAfter!.InvitedByUserId);

        // Assert - referrer's referral count is incremented
        var referrerStatsAfter = await statsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(referrerStatsAfter);
        Assert.Equal(referrerStatsBefore!.ReferralCount + 1, referrerStatsAfter!.ReferralCount);
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws InvalidOperationException when the new user has already been invited.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_NewUserAlreadyInvited_ThrowsInvalidOperationException()
    {
        // Arrange - user 1004 is seeded with InvitedByUserId = 1 (invited by Alice)
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        var referralCode = EncodeReferralCode(1001);

        var newUserStatsBefore = await statsRepository.GetStatsByUserIdAsync(1004, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(newUserStatsBefore);
        Assert.Equal(1, newUserStatsBefore!.InvitedByUserId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.ProcessReferralAsync(1004, AuthSystem.Tg, referralCode, CancellationToken.None));

        // Assert - invitation is not overwritten
        var newUserStatsAfter = await statsRepository.GetStatsByUserIdAsync(1004, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(newUserStatsAfter);
        Assert.Equal(1, newUserStatsAfter!.InvitedByUserId);
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync links the new user when the referrer has no stats yet and does not fail.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_ReferrerWithoutStats_DoesNotThrow()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();

        var createdUserId = await userRepository.CreateUserAsync(
            new UserCreateDto
            {
                ExternalUserId = 80000,
                SystemType = (byte)AuthSystem.Tg,
                FirstName = "Referrer",
                LastName = "NoStats",
                UserName = "referrer.no_stats",
                CreatedAt = DateTimeOffset.UtcNow
            },
            CancellationToken.None);

        var referrer = await userRepository.GetUserAsync(80000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(referrer);
        Assert.Equal(createdUserId, referrer.Id);

        // Referrer needs an account for the referral bonus transaction
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var createdAccountId = await accountRepository.CreateAccountAsync(
            new AccountCreateDto
            {
                ExternalUserId = 80000,
                SystemType = (byte)AuthSystem.Tg,
                Balance = 0.0m,
                CreatedAt = DateTimeOffset.UtcNow,
                Status = AccountStatus.Active
            },
            CancellationToken.None);

        var referrerStatsBefore = await statsRepository.GetStatsByUserIdAsync(80000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.Null(referrerStatsBefore);

        var referralCode = EncodeReferralCode(80000);

        var newUserStatsBefore = await statsRepository.GetStatsByUserIdAsync(1003, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.Null(newUserStatsBefore!.InvitedByUserId);

        // Act
        await handler.ProcessReferralAsync(1003, AuthSystem.Tg, referralCode, CancellationToken.None);

        // Assert - new user is linked to the referrer without stats
        var newUserStatsAfter = await statsRepository.GetStatsByUserIdAsync(1003, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(newUserStatsAfter);
        Assert.Equal(referrer.Id, newUserStatsAfter!.InvitedByUserId);

        // Assert - referral bonus transaction is credited to the referrer's account
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(createdAccountId, CancellationToken.None);
        Assert.NotNull(transactions);
        var referralTransaction = Assert.Single(transactions, t => t.CreditAccountId == createdAccountId);
        Assert.Equal(AccountConsts.SystemAccountId, referralTransaction.DebitAccountId);
        Assert.Equal(5000.00m, referralTransaction.Amount);
        Assert.Equal(BonusType.Streak, referralTransaction.BounusType);
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws InvalidOperationException when the new user has no stats yet.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_NewUserWithoutStats_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();

        var newUserId = await userRepository.CreateUserAsync(
            new UserCreateDto
            {
                ExternalUserId = 85000,
                SystemType = (byte)AuthSystem.Tg,
                FirstName = "New",
                LastName = "NoStats",
                UserName = "new.no_stats",
                CreatedAt = DateTimeOffset.UtcNow
            },
            CancellationToken.None);

        var newUserStatsBefore = await statsRepository.GetStatsByUserIdAsync(85000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.Null(newUserStatsBefore);

        var referralCode = EncodeReferralCode(1001);
        var referrerStatsBefore = await statsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.ProcessReferralAsync(85000, AuthSystem.Tg, referralCode, CancellationToken.None));

        // Assert - no stats were created for the new user
        var newUserStatsAfter = await statsRepository.GetStatsByUserIdAsync(85000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.Null(newUserStatsAfter);

        // Assert - referrer's referral count is not incremented
        var referrerStatsAfter = await statsRepository.GetStatsByUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(referrerStatsAfter);
        Assert.Equal(referrerStatsBefore!.ReferralCount, referrerStatsAfter!.ReferralCount);
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync throws InvalidOperationException when the referrer has no account.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_ReferrerWithoutAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();

        // Referrer without an account (only user is created)
        await userRepository.CreateUserAsync(
            new UserCreateDto
            {
                ExternalUserId = 86000,
                SystemType = (byte)AuthSystem.Tg,
                FirstName = "Referrer",
                LastName = "NoAccount",
                UserName = "referrer.no_account",
                CreatedAt = DateTimeOffset.UtcNow
            },
            CancellationToken.None);

        // Fresh new user with stats so the test does not depend on execution order
        await userRepository.CreateUserAsync(
            new UserCreateDto
            {
                ExternalUserId = 88000,
                SystemType = (byte)AuthSystem.Tg,
                FirstName = "New",
                LastName = "NoAccount",
                UserName = "new.no_account_referrer",
                CreatedAt = DateTimeOffset.UtcNow
            },
            CancellationToken.None);

        await statsRepository.CreateStatsAsync(
            new UserStatsCreateDto { ExternalUserId = 88000 },
            CancellationToken.None);

        var referralCode = EncodeReferralCode(86000);

        var newUserStatsBefore = await statsRepository.GetStatsByUserIdAsync(88000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.Null(newUserStatsBefore!.InvitedByUserId);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.ProcessReferralAsync(88000, AuthSystem.Tg, referralCode, CancellationToken.None));

        // Assert - new user is not linked to the referrer
        var newUserStatsAfter = await statsRepository.GetStatsByUserIdAsync(88000, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(newUserStatsAfter);
        Assert.Null(newUserStatsAfter!.InvitedByUserId);
    }

    /// <summary>
    /// Verifies that ProcessReferralAsync creates a referral bonus transaction credited to the referrer's account.
    /// </summary>
    [Fact]
    public async Task ProcessReferralAsync_ValidReferral_CreatesReferralBonusTransaction()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var statsRepository = _currentScope.ServiceProvider.GetRequiredService<IUserStatsRepository>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Fresh new user with stats so the test does not depend on execution order
        await userRepository.CreateUserAsync(
            new UserCreateDto
            {
                ExternalUserId = 87000,
                SystemType = (byte)AuthSystem.Tg,
                FirstName = "New",
                LastName = "Transaction",
                UserName = "new.transaction",
                CreatedAt = DateTimeOffset.UtcNow
            },
            CancellationToken.None);

        await statsRepository.CreateStatsAsync(
            new UserStatsCreateDto { ExternalUserId = 87000 },
            CancellationToken.None);

        // Referrer is Alice (1001) with seeded account Id = 2
        var referrerAccount = await accountRepository.GetAccountByExternalUserIdAsync(
            1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(referrerAccount);

        var transactionsBefore = await transactionRepository.GetTransactionsByAccountIdAsync(
            referrerAccount.Id, CancellationToken.None) ?? [];
        var referralTransactionsBefore = transactionsBefore.Count(t => t.BounusType == BonusType.Streak && t.Amount == 5000.00m);

        var referralCode = EncodeReferralCode(1001);

        // Act
        await handler.ProcessReferralAsync(87000, AuthSystem.Tg, referralCode, CancellationToken.None);

        // Assert - a referral bonus transaction was created for the referrer's account
        var transactionsAfter = await transactionRepository.GetTransactionsByAccountIdAsync(
            referrerAccount.Id, CancellationToken.None);
        Assert.NotNull(transactionsAfter);
        var referralTransaction = Assert.Single(transactionsAfter, t =>
            t.BounusType == BonusType.Streak
            && t.Amount == 5000.00m
            && t.CreditAccountId == referrerAccount.Id
            && t.DebitAccountId == AccountConsts.SystemAccountId);

        Assert.Equal("Streak bonus", referralTransaction.Description);

        // Assert - exactly one new referral transaction was added
        var referralTransactionsAfter = transactionsAfter.Count(t => t.BounusType == BonusType.Streak && t.Amount == 5000.00m);
        Assert.Equal(referralTransactionsBefore + 1, referralTransactionsAfter);
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
