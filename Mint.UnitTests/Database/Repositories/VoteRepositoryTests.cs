using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="VoteRepository"/>
/// </summary>
public class VoteRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public VoteRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that creating a vote returns a valid duel ID.
    /// </summary>
    [Fact]
    public async Task CreateVoteAsync_CreatedVote_ReturnsDuelId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        // Create a duel first
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#ТехноИИ",
            Question = "ИИ заменит программистов?",
            Description = "Обсуждение влияния ИИ на разработку",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        var vote = new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 1,
            OptionChosen = "A",
            BetAmount = 10.50m
        };

        // Act
        var result = await voteRepository.CreateVoteAsync(vote, CancellationToken.None);

        // Assert
        Assert.Equal(duelId, result);
    }

    /// <summary>
    /// Verifies that creating a vote with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateVoteAsync_NullVote_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateVoteAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that retrieving a vote by duel ID and account ID returns a valid VoteDto.
    /// </summary>
    [Fact]
    public async Task GetVoteAsync_ExistingVote_ReturnsVoteDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Мемы",
            Question = "Лучший мем года?",
            Description = "Голосование за лучший мем",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(48)
        }, CancellationToken.None);

        var vote = new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 1,
            OptionChosen = "B",
            BetAmount = 25.00m
        };

        await voteRepository.CreateVoteAsync(vote, CancellationToken.None);

        // Act
        var result = await voteRepository.GetVoteAsync(duelId, 1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(duelId, result.DuelId);
        Assert.Equal(1, result.AccountId);
        Assert.Equal("B", result.OptionChosen);
        Assert.Equal(25.00m, result.BetAmount);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent vote returns null.
    /// </summary>
    [Fact]
    public async Task GetVoteAsync_NonExistentVote_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Act
        var result = await repository.GetVoteAsync(0, 0, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that retrieving votes by duel ID returns the correct list.
    /// </summary>
    [Fact]
    public async Task GetVotesByDuelIdAsync_ExistingDuel_ReturnsVoteList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Наука",
            Question = "Вопрос по науке",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 1,
            OptionChosen = "A",
            BetAmount = 10.00m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            OptionChosen = "B",
            BetAmount = 20.00m
        }, CancellationToken.None);

        // Act
        var result = await voteRepository.GetVotesByDuelIdAsync(duelId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Verifies that retrieving votes for a duel without votes returns empty list.
    /// </summary>
    [Fact]
    public async Task GetVotesByDuelIdAsync_DuelWithoutVotes_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Пустой",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        // Act
        var result = await voteRepository.GetVotesByDuelIdAsync(duelId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that HasAccountVotedAsync returns true when account has voted.
    /// </summary>
    [Fact]
    public async Task HasAccountVotedAsync_AccountHasVoted_ReturnsTrue()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Тест",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 1,
            OptionChosen = "A",
            BetAmount = 5.00m
        }, CancellationToken.None);

        // Act
        var result = await voteRepository.HasAccountVotedAsync(duelId, 1, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that HasAccountVotedAsync returns false when account has not voted.
    /// </summary>
    [Fact]
    public async Task HasAccountVotedAsync_AccountHasNotVoted_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Тест",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        // Act
        var result = await voteRepository.HasAccountVotedAsync(duelId, 999999, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that votes are ordered by CreatedAt descending.
    /// </summary>
    [Fact]
    public async Task GetVotesByDuelIdAsync_ReturnsOrderedByCreatedAtDescending()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Сортировка",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 1,
            OptionChosen = "A",
            BetAmount = 10.00m
        }, CancellationToken.None);

        await Task.Delay(10); // Ensure different timestamps

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            OptionChosen = "B",
            BetAmount = 20.00m
        }, CancellationToken.None);

        // Act
        var result = await voteRepository.GetVotesByDuelIdAsync(duelId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(2, result[0].AccountId);
        Assert.Equal(1, result[1].AccountId);
    }

    /// <summary>
    /// Verifies that multiple accounts can vote in the same duel.
    /// </summary>
    [Fact]
    public async Task GetVotesByDuelIdAsync_MultipleAccountsCanVote_ReturnsAllVotes()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Мультиголосование",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 1,
            OptionChosen = "A",
            BetAmount = 10.00m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            OptionChosen = "A",
            BetAmount = 15.00m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 3,
            OptionChosen = "B",
            BetAmount = 20.00m
        }, CancellationToken.None);

        // Act
        var result = await voteRepository.GetVotesByDuelIdAsync(duelId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    /// <summary>
    /// Verifies that vote stores correct CreatedAt timestamp.
    /// </summary>
    [Fact]
    public async Task CreateVoteAsync_StoresCreatedAtTimestamp()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Время",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        var vote = new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 1,
            OptionChosen = "A",
            BetAmount = 10.00m
        };

        // Act
        await voteRepository.CreateVoteAsync(vote, CancellationToken.None);
        var result = await voteRepository.GetVoteAsync(duelId, 1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.CreatedAt > DateTimeOffset.UtcNow.AddMinutes(-5));
        Assert.True(result.CreatedAt <= DateTimeOffset.UtcNow);
    }

    /// <summary>
    /// Verifies that different accounts can vote with different options.
    /// </summary>
    [Fact]
    public async Task GetVotesByDuelIdAsync_DifferentOptionsStoredCorrectly()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var duelRepository = scope.ServiceProvider.GetRequiredService<IDuelRepository>();
        
        var duelId = await duelRepository.CreateDuelAsync(new DuelCreateDto
        {
            Category = "#Опции",
            Question = "Вопрос",
            Description = "Описание",
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 1,
            OptionChosen = "A",
            BetAmount = 10.00m
        }, CancellationToken.None);

        await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = duelId,
            AccountId = 2,
            OptionChosen = "B",
            BetAmount = 20.00m
        }, CancellationToken.None);

        // Act
        var result = await voteRepository.GetVotesByDuelIdAsync(duelId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("A", result.First(v => v.AccountId == 1).OptionChosen);
        Assert.Equal("B", result.First(v => v.AccountId == 2).OptionChosen);
    }
}
