using Mint.Database;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="DuelRepository"/>
/// </summary>
public class DuelRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public DuelRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that creating a duel returns a valid duel ID.
    /// </summary>
    [Fact]
    public async Task CreateDuelAsync_CreatedDuel_ReturnsDuelId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var duel = new DuelCreateDto
        {
            Category = "#ТехноИИ",
            Question = "ИИ заменит программистов?",
            Description = "Обсуждение влияния ИИ на разработку",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        };

        // Act
        var duelId = await repository.CreateDuelAsync(duel, CancellationToken.None);

        // Assert
        Assert.True(duelId > 0);
    }

    /// <summary>
    /// Verifies that creating a duel with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateDuelAsync_NullDuel_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateDuelAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that retrieving a duel by ID returns a valid DuelDto.
    /// </summary>
    [Fact]
    public async Task GetDuelByIdAsync_ExistingDuel_ReturnsDuelDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var duel = new DuelCreateDto
        {
            Category = "#Мемы",
            Question = "Лучший мем года?",
            Description = "Голосование за лучший мем",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(48)
        };

        // Act
        var duelId = await repository.CreateDuelAsync(duel, CancellationToken.None);
        var result = await repository.GetDuelByIdAsync(duelId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(duelId, result.Id);
        Assert.Equal("#Мемы", result.Category);
        Assert.Equal("Лучший мем года?", result.Question);
        Assert.Equal("Голосование за лучший мем", result.Description);
        Assert.False(result.IsClosed);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent duel returns null.
    /// </summary>
    [Fact]
    public async Task GetDuelByIdAsync_NonExistentDuel_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();

        // Act
        var result = await repository.GetDuelByIdAsync(0, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that retrieving active duels returns only active ones.
    /// </summary>
    [Fact]
    public async Task GetActiveDuelsAsync_ReturnsOnlyActiveDuels()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#ТехноИИ",
            Question = "Вопрос 1",
            Description = "Описание 1",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Наука",
            Question = "Вопрос 2",
            Description = "Описание 2",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(48)
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Verifies that retrieving active duels excludes expired duels.
    /// </summary>
    [Fact]
    public async Task GetActiveDuelsAsync_ExcludesExpiredDuels()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Актуальный",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Истекший",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1)
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("#Актуальный", result[0].Category);
    }

    /// <summary>
    /// Verifies that retrieving active duels excludes closed duels.
    /// </summary>
    [Fact]
    public async Task GetActiveDuelsAsync_ExcludesClosedDuels()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Активный",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        // Create a closed duel by manually setting it
        var closedDuelId = await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Закрытый",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(48)
        }, CancellationToken.None);

        // Manually close the duel via DbContext
        using var scope2 = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope2.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var closedDuel = await context.Duels.FindAsync(closedDuelId);
        if (closedDuel is not null)
        {
            closedDuel.IsClosed = true;
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // Act
        var result = await repository.GetActiveDuelsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("#Активный", result[0].Category);
    }

    /// <summary>
    /// Verifies that active duels are ordered by ID descending.
    /// </summary>
    [Fact]
    public async Task GetActiveDuelsAsync_ReturnsOrderedByIdDescending()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Первый",
            Question = "Вопрос 1",
            Description = "Описание 1",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Второй",
            Question = "Вопрос 2",
            Description = "Описание 2",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("#Второй", result[0].Category);
        Assert.Equal("#Первый", result[1].Category);
    }

    /// <summary>
    /// Verifies that retrieving active duels when none exist returns empty list.
    /// </summary>
    [Fact]
    public async Task GetActiveDuelsAsync_NoActiveDuels_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        // Create only expired duels
        await repository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Истекший",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1)
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
