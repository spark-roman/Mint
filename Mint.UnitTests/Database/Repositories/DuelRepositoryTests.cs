using Mint.Common.Contracts.UserInteractive;
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
        await _fixture.ResetAsync(CancellationToken.None);

        var duel = new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "ИИ заменит программистов?",
            Description = "Обсуждение влияния ИИ на разработку",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = new[]
            {
                new DuelOptionCreateDto { OptionText = "Да", OptionCode = "yes" },
                new DuelOptionCreateDto { OptionText = "Нет", OptionCode = "no" }
            }
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
        await _fixture.ResetAsync(CancellationToken.None);

        var duel = new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Лучший мем года?",
            Description = "Голосование за лучший мем",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(48),
            Options = new[]
            {
                new DuelOptionCreateDto { OptionText = "Мем 1", OptionCode = "mem1" },
                new DuelOptionCreateDto { OptionText = "Мем 2", OptionCode = "mem2" }
            }
        };

        // Act
        var duelId = await repository.CreateDuelAsync(duel, CancellationToken.None);
        var result = await repository.GetDuelByIdAsync(duelId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(duelId, result.Id);
        Assert.Equal(1, result.CategoryId);
        Assert.Equal(DuelType.OpinionMatch, result.DuelType);
        Assert.Equal("Лучший мем года?", result.Question);
        Assert.Equal("Голосование за лучший мем", result.Description);
        Assert.False(result.IsClosed);
        Assert.Equal(2, result.Options.Count());
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
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос 1",
            Description = "Описание 1",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = [new() { OptionText = "Опция 1", OptionCode = "opt1" }]
        }, CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 2,
            DuelType = DuelType.FactPrediction,
            Question = "Вопрос 2",
            Description = "Описание 2",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(48),
            Options = [new() { OptionText = "Опция 2", OptionCode = "opt2" }]
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
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = [new() { OptionText = "Актуальный", OptionCode = "actual" }]
        }, CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1),
            Options = [new() { OptionText = "Истекший", OptionCode = "expired" }]
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Вопрос", result[0].Question);
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
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = [new() { OptionText = "Активный", OptionCode = "active" }]
        }, CancellationToken.None);

        // Create a closed duel by manually setting it
        var closedDuelId = await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(48),
            Options = [new() { OptionText = "Закрытый", OptionCode = "closed" }]
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
        Assert.Equal("Вопрос", result[0].Question);
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
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос 1",
            Description = "Описание 1",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = [new() { OptionText = "Первый", OptionCode = "first" }]
        }, CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос 2",
            Description = "Описание 2",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = [new() { OptionText = "Второй", OptionCode = "second" }]
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Вопрос 2", result[0].Question);
        Assert.Equal("Вопрос 1", result[1].Question);
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
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1),
            Options = [new() { OptionText = "Истекший", OptionCode = "expired" }]
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
