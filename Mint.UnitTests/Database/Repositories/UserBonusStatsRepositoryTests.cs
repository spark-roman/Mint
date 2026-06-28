using Mint.Database.Entities.UserInteractive.Bonuses.Dto;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="UserBonusStatsRepository"/>
/// </summary>
public class UserBonusStatsRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public UserBonusStatsRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that creating user bonus stats returns a valid stats ID.
    /// </summary>
    [Fact]
    public async Task CreateStatsAsync_CreatedStats_ReturnsStatsId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var stats = new UserBonusStatsCreateDto
        {
            InternalUserId = 1,
            ExternalUserId = 1001,
            IsStartBonusClaimed = false,
            CurrentDailyStreak = 0,
            TotalReferralBonusesClaimed = 0
        };

        // Act
        var statsId = await repository.CreateStatsAsync(stats, CancellationToken.None);

        // Assert
        Assert.True(statsId > 0);
    }

    /// <summary>
    /// Verifies that creating user bonus stats with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateStatsAsync_NullStats_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateStatsAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that retrieving user bonus stats by user ID returns a valid UserBonusStatsDto.
    /// </summary>
    [Fact]
    public async Task GetStatsByUserIdAsync_ExistingStats_ReturnsStatsDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var now = DateTimeOffset.UtcNow;
        var stats = new UserBonusStatsCreateDto
        {
            InternalUserId = 1,
            ExternalUserId = 1001,
            IsStartBonusClaimed = true,
            CurrentDailyStreak = 5,
            LastDailyClaimedAt = now.AddDays(-1),
            NextDailyAvailableAt = now,
            TotalReferralBonusesClaimed = 3
        };

        // Act
        var statsId = await repository.CreateStatsAsync(stats, CancellationToken.None);
        var result = await repository.GetStatsByUserIdAsync(stats.ExternalUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(statsId, result.Id);
        Assert.Equal(1, result.InternalUserId);
        Assert.True(result.IsStartBonusClaimed);
        Assert.Equal(5, result.CurrentDailyStreak);
        Assert.Equal(3, result.TotalReferralBonusesClaimed);
    }

    /// <summary>
    /// Verifies that retrieving non-existent user bonus stats returns null.
    /// </summary>
    [Fact]
    public async Task GetStatsByUserIdAsync_NonExistentStats_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();

        // Act
        var result = await repository.GetStatsByUserIdAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that updating user bonus stats returns true and updates the data.
    /// </summary>
    [Fact]
    public async Task UpdateStatsAsync_ExistingStats_UpdatesSuccessfully()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var createStats = new UserBonusStatsCreateDto
        {
            InternalUserId = 1,
            ExternalUserId = 1001,
            IsStartBonusClaimed = false,
            CurrentDailyStreak = 2,
            TotalReferralBonusesClaimed = 1
        };

        await repository.CreateStatsAsync(createStats, CancellationToken.None);

        var now = DateTimeOffset.UtcNow;
        var updateStats = new UserBonusStatsUpdateDto
        {
            InternalUserId = 1,
            ExternalUserId = 1001,
            IsStartBonusClaimed = true,
            CurrentDailyStreak = 3,
            LastDailyClaimedAt = now,
            NextDailyAvailableAt = now.AddDays(1),
            TotalReferralBonusesClaimed = 2
        };

        // Act
        var result = await repository.UpdateStatsAsync(updateStats, CancellationToken.None);
        var updated = await repository.GetStatsByUserIdAsync(updateStats.ExternalUserId, CancellationToken.None);

        // Assert
        Assert.True(result);
        Assert.NotNull(updated);
        Assert.True(updated.IsStartBonusClaimed);
        Assert.Equal(3, updated.CurrentDailyStreak);
        Assert.Equal(2, updated.TotalReferralBonusesClaimed);
    }

    /// <summary>
    /// Verifies that updating non-existent user bonus stats returns false.
    /// </summary>
    [Fact]
    public async Task UpdateStatsAsync_NonExistentStats_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var updateStats = new UserBonusStatsUpdateDto
        {
            InternalUserId = 1,
            ExternalUserId = 999,
            IsStartBonusClaimed = true,
            CurrentDailyStreak = 5
        };

        // Act
        var result = await repository.UpdateStatsAsync(updateStats, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that updating user bonus stats with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task UpdateStatsAsync_NullStats_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.UpdateStatsAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that creating bonus stats for different users returns different IDs.
    /// </summary>
    [Fact]
    public async Task CreateStatsAsync_DifferentUsers_ReturnsDifferentIds()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserBonusStatsRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var stats1 = new UserBonusStatsCreateDto
        {
            InternalUserId = 1,
            ExternalUserId = 1001,
            IsStartBonusClaimed = false,
            CurrentDailyStreak = 0
        };

        var stats2 = new UserBonusStatsCreateDto
        {
            InternalUserId = 2,
            ExternalUserId = 1002,
            IsStartBonusClaimed = true,
            CurrentDailyStreak = 5
        };

        // Act
        var statsId1 = await repository.CreateStatsAsync(stats1, CancellationToken.None);
        var statsId2 = await repository.CreateStatsAsync(stats2, CancellationToken.None);

        // Assert
        Assert.NotEqual(statsId1, statsId2);
        Assert.True(statsId1 > 0);
        Assert.True(statsId2 > 0);
    }
}
