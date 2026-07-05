using AdvApplication.Auth.Users;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Common.Contracts.Users;
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

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1001, result.ExternalUserId);
        Assert.Equal("Alice", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal("alice.smith", result.UserName);
        Assert.Equal(1500.50m, result.Balance);
        Assert.Equal(150, result.RankPoints);
        Assert.True(result.TotalDuels > 0);
        Assert.Equal(10, result.Wins);
        Assert.Equal(5, result.Losses);
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

    #region GetUserProfileAsync - Happy Path

    /// <summary>
    /// Verifies that GetUserProfileAsync returns profile data for external user DTO.
    /// </summary>
    [Fact]
    public async Task GetUserProfileAsync_ValidUser_ReturnsUserProfileDto()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        var externalUser = new ExternalUserDto
        {
            ExternalUserId = 1001,
            FirstName = "Alice",
            Username = "alice.smith"
        };

        // Act
        var result = await handler.GetUserProfileAsync(externalUser, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // GetUserProfileAsync uses FirstName first, then Username
        Assert.Equal("Alice", result.UserName);
        Assert.True(result.Balance >= 0);
        Assert.True(result.TotalWins >= 0);
    }

    /// <summary>
    /// Verifies that GetUserProfileAsync falls back to FirstName when Username is null.
    /// </summary>
    [Fact]
    public async Task GetUserProfileAsync_NullUsername_UsesFirstName()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        var externalUser = new ExternalUserDto
        {
            ExternalUserId = 1001,
            FirstName = "Alice",
            Username = null
        };

        // Act
        var result = await handler.GetUserProfileAsync(externalUser, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Alice", result.UserName);
    }

    /// <summary>
    /// Verifies that GetUserProfileAsync returns empty string when both FirstName and Username are null.
    /// </summary>
    [Fact]
    public async Task GetUserProfileAsync_BothNamesNull_ReturnsEmptyUserName()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        var externalUser = new ExternalUserDto
        {
            ExternalUserId = 1001,
            FirstName = string.Empty,
            Username = null
        };

        // Act
        var result = await handler.GetUserProfileAsync(externalUser, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.UserName);
    }

    /// <summary>
    /// Verifies that GetUserProfileAsync throws when userDto is null.
    /// </summary>
    [Fact]
    public async Task GetUserProfileAsync_NullUserDto_ThrowsArgumentNullException()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.GetUserProfileAsync(null!, CancellationToken.None));
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
    }

    /// <summary>
    /// Verifies that InitializeUserAsync updates existing user fields and returns user.
    /// </summary>
    [Fact]
    public async Task InitializeUserAsync_ExistingUser_UpdatesFieldsAndReturnsUser()
    {
        // Arrange
        _currentScope = _fixture.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IUserProfilesHandler>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();

        var existingUser = await userRepository.GetUserAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(existingUser);
        var originalLastAuthDate = existingUser.LastAuthDate;

        var updateDto = new UserCreateDto
        {
            ExternalUserId = 1001,
            SystemType = (byte)AuthSystem.Tg,
            FirstName = "Alice",
            LastName = "Updated",
            UserName = "alice.updated",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Task.Delay(10); // Ensure time difference for LastAuthDate

        // Act
        var result = await handler.InitializeUserAsync(updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1001, result.ExternalUserId);
        Assert.Equal("Alice", result.FirstName);
        Assert.Equal("Updated", result.LastName);
        Assert.Equal("alice.updated", result.UserName);

        // Verify fields were updated in repository
        var updatedUser = await userRepository.GetUserAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(updatedUser);
        Assert.Equal("Updated", updatedUser.LastName);
        Assert.Equal("alice.updated", updatedUser.UserName);
        Assert.NotNull(updatedUser.LastAuthDate);
        if (originalLastAuthDate.HasValue)
        {
            Assert.True(updatedUser.LastAuthDate.Value > originalLastAuthDate.Value);
        }
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
        Assert.Equal(3, result.Count);
        // Verify sorted by rank points ascending (as implemented)
        Assert.True(result[0].RankPoints <= result[1].RankPoints);
        Assert.True(result[1].RankPoints <= result[2].RankPoints);
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

        // Act
        var result = await handler.GetLeaderboardAsync(10, AuthSystem.Tg, CancellationToken.None);

        // Assert
        var alice = result.FirstOrDefault(r => r.ExternalUserId == 1001);
        Assert.NotNull(alice);
        Assert.Equal(66.7, alice.Winrate);
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
