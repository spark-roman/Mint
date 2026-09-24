using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Users;
using Mint.Database;
using Mint.Database.Entities.Users;

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

        var webStats = new UserStatsCreateDto
        {
            ExternalUserId = 1001,
            RankPoints = 10,
            TotalWins = 1,
            TotalLosses = 0
        };
        await repository.CreateStatsAsync(webStats, CancellationToken.None);

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
}
