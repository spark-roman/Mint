using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Users;

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
        var result = await repository.UpdateStatsAsync(createStats.ExternalUserId, updateStats, CancellationToken.None);
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
        var result = await repository.UpdateStatsAsync(999, updateStats, CancellationToken.None);

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
            await repository.UpdateStatsAsync(1, null!, CancellationToken.None));
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
}
