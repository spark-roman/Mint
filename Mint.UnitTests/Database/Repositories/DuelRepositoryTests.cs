using Mint.Common.Contracts.UserInteractive;
using Mint.Database;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.UserInteractive.Duels;

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
        Assert.Equal(DuelStatus.Active, result.Status);
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
    public async Task GetActiveDuelsForCloseAsync_ReturnsOnlyActiveDuels()
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
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-24),
            Options = [new() { OptionText = "Опция 1", OptionCode = "opt1" }]
        }, CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 2,
            DuelType = DuelType.FactPrediction,
            Question = "Вопрос 2",
            Description = "Описание 2",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-48),
            Options = [new() { OptionText = "Опция 2", OptionCode = "opt2" }]
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsForCloseAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Verifies that retrieving active duels excludes expired duels.
    /// </summary>
    [Fact]
    public async Task GetActiveDuelsForCloseAsync_ExcludesExpiredDuels()
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
        var result = await repository.GetActiveDuelsForCloseAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Вопрос", result[0].Question);
    }

    /// <summary>
    /// Verifies that retrieving active duels excludes closed duels.
    /// </summary>
    [Fact]
    public async Task GetActiveDuelsForCloseAsync_ExcludesClosedDuels()
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
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-24),
            Options = [new() { OptionText = "Активный", OptionCode = "active" }]
        }, CancellationToken.None);

        // Create a closed duel by manually setting it
        var closedDuelId = await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-48),
            Options = [new() { OptionText = "Закрытый", OptionCode = "closed" }]
        }, CancellationToken.None);

        // Manually close the duel via DbContext
        using var scope2 = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope2.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var closedDuel = await context.Duels.FindAsync(closedDuelId);
        if (closedDuel is not null)
        {
            closedDuel.Status = DuelStatus.Closed;
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // Act
        var result = await repository.GetActiveDuelsForCloseAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Вопрос", result[0].Question);
    }

    /// <summary>
    /// Verifies that active duels are ordered by ID descending.
    /// </summary>
    [Fact]
    public async Task GetActiveDuelsForCloseAsync_ReturnsOrderedByIdDescending()
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
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-24),
            Options = [new() { OptionText = "Первый", OptionCode = "first" }]
        }, CancellationToken.None);

        await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос 2",
            Description = "Описание 2",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-24),
            Options = [new() { OptionText = "Второй", OptionCode = "second" }]
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsForCloseAsync(CancellationToken.None);

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
    public async Task GetActiveDuelsForCloseAsync_NoActiveDuels_ReturnsEmptyList()
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
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = [new() { OptionText = "Истекший", OptionCode = "expired" }]
        }, CancellationToken.None);

        // Act
        var result = await repository.GetActiveDuelsForCloseAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that GetFirstAvailableDuelAsync returns null when user has already voted in a duel in the category.
    /// </summary>
    [Fact]
    public async Task GetFirstAvailableDuelAsync_UserAlreadyVoted_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        // User with AccountId = 1 votes in the duel
        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 1,
            BetAmount = 10.00m
        }, CancellationToken.None);

        // Act
        var result = await repository.GetFirstAvailableDuelAsync(1, accountId: 2, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that GetFirstAvailableDuelAsync returns the duel when user has not voted.
    /// </summary>
    [Fact]
    public async Task GetFirstAvailableDuelAsync_UserHasNotVoted_ReturnsDuel()
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
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(48),
            Options = [new() { OptionText = "Опция", OptionCode = "opt" }]
        }, CancellationToken.None);

        // Act
        var result = await repository.GetFirstAvailableDuelAsync(1, accountId: 999, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.CategoryId);
        Assert.Equal("Вопрос", result.Question);
    }

    /// <summary>
    /// Verifies that GetFirstAvailableDuelAsync returns null when no open duels exist in the category.
    /// </summary>
    [Fact]
    public async Task GetFirstAvailableDuelAsync_NoDuelsInCategory_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        // Act
        var result = await repository.GetFirstAvailableDuelAsync(999, accountId: 1, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that GetFirstAvailableDuelAsync excludes expired duels.
    /// </summary>
    [Fact]
    public async Task GetFirstAvailableDuelAsync_ExpiredDuel_ReturnsNull()
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
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1),
            Options = [new() { OptionText = "Опция", OptionCode = "opt" }]
        }, CancellationToken.None);

        // Act
        var result = await repository.GetFirstAvailableDuelAsync(1, accountId: 1, CancellationToken.None);

        // Assert
        Assert.Equal(100500, result!.Id);
    }

    #region PublishDuelsAsync

    /// <summary>
    /// Verifies that PublishDuelsAsync returns 0 when no planned duels exist.
    /// </summary>
    [Fact]
    public async Task PublishDuelsAsync_NoPlannedDuels_ReturnsZero()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var expiresAt = DateTimeOffset.UtcNow.AddHours(24);

        // Act
        var result = await repository.PublishDuelsAsync(expiresAt, CancellationToken.None);

        // Assert
        Assert.Equal(0, result);
    }

    /// <summary>
    /// Verifies that PublishDuelsAsync publishes planned duels and returns correct count.
    /// </summary>
    [Fact]
    public async Task PublishDuelsAsync_PlannedDuels_ReturnsCount()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await _fixture.ResetAsync(CancellationToken.None);

        var expiresAt = DateTimeOffset.UtcNow.AddHours(48);

        // Create active duel (should be ignored)
        await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Активная дуэль",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = [new() { OptionText = "Опция", OptionCode = "opt" }]
        }, CancellationToken.None);

        // Create planned duels manually
        await using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var plannedDuel1 = new DuelEntity
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Планируемая дуэль 1",
            Description = "Описание 1",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1),
            Status = DuelStatus.Planned,
            Options = [new() { OptionText = "Опция", OptionCode = "opt1" }]
        };
        var plannedDuel2 = new DuelEntity
        {
            CategoryId = 2,
            DuelType = DuelType.FactPrediction,
            Question = "Планируемая дуэль 2",
            Description = "Описание 2",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-2),
            Status = DuelStatus.Planned,
            Options = [new() { OptionText = "Опция", OptionCode = "opt2" }]
        };

        context.Duels.AddRange(plannedDuel1, plannedDuel2);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.PublishDuelsAsync(expiresAt, CancellationToken.None);

        // Assert
        Assert.Equal(2, result);

        // Verify duels were updated
        var updatedDuel1 = await repository.GetDuelByIdAsync(plannedDuel1.Id, CancellationToken.None);
        var updatedDuel2 = await repository.GetDuelByIdAsync(plannedDuel2.Id, CancellationToken.None);

        Assert.NotNull(updatedDuel1);
        Assert.NotNull(updatedDuel2);
        Assert.Equal(DuelStatus.Closed, updatedDuel1.Status);
        Assert.Equal(DuelStatus.Closed, updatedDuel2.Status);
        Assert.Equal(expiresAt, updatedDuel1.ExpiresAt);
        Assert.Equal(expiresAt, updatedDuel2.ExpiresAt);
    }

    /// <summary>
    /// Verifies that PublishDuelsAsync does not affect active duels.
    /// </summary>
    [Fact]
    public async Task PublishDuelsAsync_ActiveDuels_NotAffected()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        await _fixture.ResetAsync(CancellationToken.None);

        var duelId = await repository.CreateDuelAsync(new DuelCreateDto
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Активная дуэль",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24),
            Options = [new() { OptionText = "Опция", OptionCode = "opt" }]
        }, CancellationToken.None);

        var originalExpiresAt = (await repository.GetDuelByIdAsync(duelId, CancellationToken.None))!.ExpiresAt;

        // Create a planned duel
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var plannedDuel = new DuelEntity
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Планируемая",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1),
            Status = DuelStatus.Planned,
            Options = [new() { OptionText = "Опция", OptionCode = "opt" }]
        };
        context.Duels.Add(plannedDuel);
        await context.SaveChangesAsync(CancellationToken.None);

        var expiresAt = DateTimeOffset.UtcNow.AddHours(48);

        // Act
        await repository.PublishDuelsAsync(expiresAt, CancellationToken.None);

        // Assert
        var activeDuel = await repository.GetDuelByIdAsync(duelId, CancellationToken.None);
        Assert.NotNull(activeDuel);
        Assert.Equal(DuelStatus.Active, activeDuel.Status);
        Assert.Equal(originalExpiresAt, activeDuel.ExpiresAt);
    }

    /// <summary>
    /// Verifies that PublishDuelsAsync does not affect closed duels.
    /// </summary>
    [Fact]
    public async Task PublishDuelsAsync_ClosedDuels_NotAffected()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        await _fixture.ResetAsync(CancellationToken.None);

        var expiresAt = DateTimeOffset.UtcNow.AddHours(48);

        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var closedDuel = new DuelEntity
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Закрытая дуэль",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-24),
            Status = DuelStatus.Closed,
            Options = [new() { OptionText = "Опция", OptionCode = "opt" }]
        };
        var plannedDuel = new DuelEntity
        {
            CategoryId = 1,
            DuelType = DuelType.OpinionMatch,
            Question = "Планируемая",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(-1),
            Status = DuelStatus.Planned,
            Options = [new() { OptionText = "Опция", OptionCode = "opt" }]
        };

        context.Duels.AddRange(closedDuel, plannedDuel);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.PublishDuelsAsync(expiresAt, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);

        var updatedClosedDuel = await repository.GetDuelByIdAsync(closedDuel.Id, CancellationToken.None);
        Assert.NotNull(updatedClosedDuel);
        Assert.Equal(DuelStatus.Closed, updatedClosedDuel.Status);
    }

    #endregion
}
