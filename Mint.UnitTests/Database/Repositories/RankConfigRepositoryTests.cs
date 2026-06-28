using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="RankConfigRepository"/>
/// </summary>
public class RankConfigRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public RankConfigRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that retrieving a rank config by ID returns a valid RankConfigDto.
    /// </summary>
    [Fact]
    public async Task GetRankConfigByIdAsync_ExistingRank_ReturnsRankConfigDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRankConfigRepository>();

        // Act
        var result = await repository.GetRankConfigByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("newbie", result.Code);
        Assert.Equal("Новичок", result.Name);
        Assert.Equal("🥚", result.Emoji);
        Assert.Equal(0, result.MinPoints);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent rank config returns null.
    /// </summary>
    [Fact]
    public async Task GetRankConfigByIdAsync_NonExistentRank_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRankConfigRepository>();

        // Act
        var result = await repository.GetRankConfigByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that retrieving a rank config by code returns a valid RankConfigDto.
    /// </summary>
    [Fact]
    public async Task GetRankConfigByCodeAsync_ExistingCode_ReturnsRankConfigDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRankConfigRepository>();

        // Act
        var result = await repository.GetRankConfigByCodeAsync("analyst", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("analyst", result.Code);
        Assert.Equal("Аналитик", result.Name);
        Assert.Equal("📈", result.Emoji);
        Assert.Equal(20, result.MinPoints);
    }

    /// <summary>
    /// Verifies that retrieving a rank config by non-existent code returns null.
    /// </summary>
    [Fact]
    public async Task GetRankConfigByCodeAsync_NonExistentCode_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRankConfigRepository>();

        // Act
        var result = await repository.GetRankConfigByCodeAsync("nonexistent", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that retrieving all rank configs returns the correct count.
    /// </summary>
    [Fact]
    public async Task GetRankConfigsAsync_ReturnsAllRankConfigs()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRankConfigRepository>();

        // Act
        var result = await repository.GetRankConfigsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
    }

    /// <summary>
    /// Verifies that rank configs are ordered by MinPoints ascending.
    /// </summary>
    [Fact]
    public async Task GetRankConfigsAsync_ReturnsOrderedByMinPoints()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRankConfigRepository>();

        // Act
        var result = await repository.GetRankConfigsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result[0].MinPoints);
        Assert.Equal(20, result[1].MinPoints);
        Assert.Equal(100, result[2].MinPoints);
        Assert.Equal(500, result[3].MinPoints);
        Assert.Equal(2000, result[4].MinPoints);
    }

    /// <summary>
    /// Verifies that getting the highest rank returns the one with the most points.
    /// </summary>
    [Fact]
    public async Task GetHighestRankAsync_ReturnsHighestRank()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRankConfigRepository>();

        // Act
        var result = await repository.GetHighestRankAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2000, result.MinPoints);
        Assert.Equal("Оракул", result.Name);
        Assert.Equal("👁️", result.Emoji);
    }

    /// <summary>
    /// Verifies that retrieving a rank config by null code throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task GetRankConfigByCodeAsync_NullCode_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRankConfigRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.GetRankConfigByCodeAsync(null!, CancellationToken.None));
    }
}
