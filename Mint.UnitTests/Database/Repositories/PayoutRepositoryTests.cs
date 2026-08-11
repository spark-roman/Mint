using System.Collections.ObjectModel;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database.Entities.System.Payouts.Dto;
using Mint.Database.Entities.System.Payouts.Repositories;
using Mint.Database.Entities.UserInteractive.Votes;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="PayoutRepository"/>
/// </summary>
public class PayoutRepositoryTests : IClassFixture<PayoutRepositoryFixture>
{
    private readonly PayoutRepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public PayoutRepositoryTests(PayoutRepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    #region CreateAsync

    /// <summary>
    /// Verifies that creating a payout returns a valid PayoutDto with a positive ID.
    /// </summary>
    [Fact]
    public async Task CreateAsync_CreatedPayout_ReturnsPayoutDto()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 100.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payout = new PayoutCreateDto
        {
            VoteId = voteId,
            DuelId = 100500,
            AccountId = 1,
            Amount = 150.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        };

        // Act
        var result = await repository.CreateAsync(payout, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
    }

    /// <summary>
    /// Verifies that creating a payout with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateAsync_NullPayout_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that creating a payout sets the correct default status (Pending).
    /// </summary>
    [Fact]
    public async Task CreateAsync_CreatedPayout_HasPendingStatus()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 50.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payout = new PayoutCreateDto
        {
            VoteId = voteId,
            DuelId = 100500,
            AccountId = 1,
            Amount = 75.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        };

        // Act
        var result = await repository.CreateAsync(payout, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PayoutStatus.Pending, result.Status);
    }

    /// <summary>
    /// Verifies that creating a payout correctly maps all fields.
    /// </summary>
    [Fact]
    public async Task CreateAsync_CorrectlyMapsFields()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var expectedDuelId = 100500L;
        var expectedAccountId = 1L;
        var expectedAmount = 42.50m;

        var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = expectedDuelId,
            AccountId = 2,
            ChosenOptionId = 100502,
            BetAmount = 10.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Act
        var result = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId,
            DuelId = expectedDuelId,
            AccountId = expectedAccountId,
            Amount = expectedAmount,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(voteId, result.VoteId);
        Assert.Equal(expectedDuelId, result.DuelId);
        Assert.Equal(expectedAccountId, result.AccountId);
        Assert.Equal(expectedAmount, result.Amount);
        Assert.Equal(PayoutStatus.Pending, result.Status);
    }

    #endregion

    #region CreateManyAsync

    /// <summary>
    /// Verifies that creating multiple payouts returns a list with correct count.
    /// </summary>
    [Fact]
    public async Task CreateManyAsync_CreatedPayouts_ReturnsCorrectCount()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId1 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 10.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var voteId2 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100502,
            BetAmount = 20.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var dtos = new ReadOnlyCollection<PayoutCreateDto>(
        [
            new PayoutCreateDto
            {
                VoteId = voteId1,
                DuelId = 100500,
                AccountId = 1,
                Amount = 15.00m,
                ProcessedAt = DateTimeOffset.UtcNow
            },
            new PayoutCreateDto
            {
                VoteId = voteId2,
                DuelId = 100500,
                AccountId = 1,
                Amount = 25.00m,
                ProcessedAt = DateTimeOffset.UtcNow
            }
        ]);

        // Act
        var results = await repository.CreateManyAsync(dtos, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }

    /// <summary>
    /// Verifies that creating multiple payouts with empty list returns empty list.
    /// </summary>
    [Fact]
    public async Task CreateManyAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var dtos = new ReadOnlyCollection<PayoutCreateDto>([]);

        // Act
        var results = await repository.CreateManyAsync(dtos, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    #endregion

    #region GetByIdAsync

    /// <summary>
    /// Verifies that retrieving a payout by ID returns a valid PayoutDto with correct data.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ExistingPayout_ReturnsPayoutDto()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 200.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payoutDto = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId,
            DuelId = 100500,
            AccountId = 1,
            Amount = 300.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Act
        var result = await repository.GetByIdAsync(payoutDto.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payoutDto.Id, result.Id);
        Assert.Equal(voteId, result.VoteId);
        Assert.Equal(100500, result.DuelId);
        Assert.Equal(1, result.AccountId);
        Assert.Equal(300.00m, result.Amount);
        Assert.Equal(PayoutStatus.Pending, result.Status);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent payout returns null.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_NonExistentPayout_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();

        // Act
        var result = await repository.GetByIdAsync(0, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region GetByAccountIdAsync

    /// <summary>
    /// Verifies that retrieving payouts by account ID returns the correct list.
    /// </summary>
    [Fact]
    public async Task GetByAccountIdAsync_ExistingAccount_ReturnsPayoutList()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId1 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 10.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var voteId2 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100502,
            BetAmount = 20.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId1,
            DuelId = 100500,
            AccountId = 1,
            Amount = 15.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId2,
            DuelId = 100500,
            AccountId = 1,
            Amount = 25.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Act
        var result = await repository.GetByAccountIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, payout => Assert.Equal(1, payout.AccountId));
    }

    /// <summary>
    /// Verifies that retrieving payouts for an account without payouts returns empty list.
    /// </summary>
    [Fact]
    public async Task GetByAccountIdAsync_AccountWithoutPayouts_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();

        // Act
        var result = await repository.GetByAccountIdAsync(999999, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region GetByDuelIdAsync

    /// <summary>
    /// Verifies that retrieving payouts by duel ID returns the correct list.
    /// </summary>
    [Fact]
    public async Task GetByDuelIdAsync_ExistingDuel_ReturnsPayoutList()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId1 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 10.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var voteId2 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100502,
            BetAmount = 20.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId1,
            DuelId = 100500,
            AccountId = 1,
            Amount = 15.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId2,
            DuelId = 100500,
            AccountId = 1,
            Amount = 25.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Act
        var result = await repository.GetByDuelIdAsync(100500, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, payout => Assert.Equal(100500, payout.DuelId));
    }

    /// <summary>
    /// Verifies that retrieving payouts for a duel without payouts returns empty list.
    /// </summary>
    [Fact]
    public async Task GetByDuelIdAsync_DuelWithoutPayouts_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();

        // Act
        var result = await repository.GetByDuelIdAsync(999999, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region UpdateAsync

    /// <summary>
    /// Verifies that updating payout status to Completed returns the updated payout.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ExistingPayout_UpdatesToCompleted_ReturnsUpdatedPayout()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 50.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payoutDto = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId,
            DuelId = 100500,
            AccountId = 1,
            Amount = 75.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var updateDto = new PayoutUpdateDto
        {
            Id = payoutDto.Id,
            Status = PayoutStatus.Completed,
            TransactionId = 1
        };

        // Act
        var result = await repository.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(payoutDto.Id, result.Id);
        Assert.Equal(PayoutStatus.Completed, result.Status);
        Assert.Equal(1, result.TransactionId);
    }

    /// <summary>
    /// Verifies that updating payout status to Failed returns the updated payout.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ExistingPayout_UpdatesToFailed_ReturnsUpdatedPayout()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 30.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payoutDto = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId,
            DuelId = 100500,
            AccountId = 1,
            Amount = 45.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var updateDto = new PayoutUpdateDto
        {
            Id = payoutDto.Id,
            Status = PayoutStatus.Failed
        };

        // Act
        var result = await repository.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PayoutStatus.Failed, result.Status);
    }

    /// <summary>
    /// Verifies that updating payout status to Cancelled returns the updated payout.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ExistingPayout_UpdatesToCancelled_ReturnsUpdatedPayout()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 10.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payoutDto = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId,
            DuelId = 100500,
            AccountId = 1,
            Amount = 15.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var updateDto = new PayoutUpdateDto
        {
            Id = payoutDto.Id,
            Status = PayoutStatus.Cancelled
        };

        // Act
        var result = await repository.UpdateAsync(updateDto, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PayoutStatus.Cancelled, result.Status);
    }

    /// <summary>
    /// Verifies that updating a non-existent payout throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_NonExistentPayout_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();

        var updateDto = new PayoutUpdateDto
        {
            Id = 0,
            Status = PayoutStatus.Completed
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.UpdateAsync(updateDto, CancellationToken.None));
        Assert.Contains("Payout 0 not found", exception.Message);
    }

    /// <summary>
    /// Verifies that passing null to UpdateAsync throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.UpdateAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that updating payout without TransactionId preserves existing TransactionId.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithoutTransactionId_PreservesExistingTransactionId()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 50.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payoutDto = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId,
            DuelId = 100500,
            AccountId = 1,
            Amount = 75.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var updateDto = new PayoutUpdateDto
        {
            Id = payoutDto.Id,
            Status = PayoutStatus.Completed,
            TransactionId = 42
        };

        // Act
        await repository.UpdateAsync(updateDto, CancellationToken.None);

        // Update without TransactionId
        var updateDtoNoTransaction = new PayoutUpdateDto
        {
            Id = payoutDto.Id,
            Status = PayoutStatus.Completed
        };

        var result = await repository.UpdateAsync(updateDtoNoTransaction, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.TransactionId);
    }

    #endregion

    #region UpdateManyAsync

    /// <summary>
    /// Verifies that updating multiple payouts returns a list with correct count.
    /// </summary>
    [Fact]
    public async Task UpdateManyAsync_UpdatedPayouts_ReturnsCorrectCount()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var voteId1 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 10.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var voteId2 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100502,
            BetAmount = 20.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payout1 = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId1,
            DuelId = 100500,
            AccountId = 1,
            Amount = 15.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payout2 = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId2,
            DuelId = 100500,
            AccountId = 1,
            Amount = 25.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var dtos = new ReadOnlyCollection<PayoutUpdateDto>(
        [
            new PayoutUpdateDto { Id = payout1.Id, Status = PayoutStatus.Completed },
            new PayoutUpdateDto { Id = payout2.Id, Status = PayoutStatus.Failed }
        ]);

        // Act
        var results = await repository.UpdateManyAsync(dtos, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
        Assert.Contains(results, p => p.Id == payout1.Id && p.Status == PayoutStatus.Completed);
        Assert.Contains(results, p => p.Id == payout2.Id && p.Status == PayoutStatus.Failed);
    }

    /// <summary>
    /// Verifies that updating multiple payouts with empty list returns empty list.
    /// </summary>
    [Fact]
    public async Task UpdateManyAsync_EmptyList_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var dtos = new ReadOnlyCollection<PayoutUpdateDto>([]);

        // Act
        var results = await repository.UpdateManyAsync(dtos, CancellationToken.None);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    #endregion

    #region GetPendingPayoutsAsync

    /// <summary>
    /// Verifies that retrieving pending payouts returns only pending ones.
    /// </summary>
    [Fact]
    public async Task GetPendingPayoutsAsync_ReturnsOnlyPendingPayouts()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Create pending payouts
        var voteId1 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 10.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var voteId2 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100502,
            BetAmount = 20.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId1,
            DuelId = 100500,
            AccountId = 1,
            Amount = 15.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId2,
            DuelId = 100500,
            AccountId = 1,
            Amount = 25.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Act
        var result = await repository.GetPendingPayoutsAsync(10, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, payout => Assert.Equal(PayoutStatus.Pending, payout.Status));
    }

    /// <summary>
    /// Verifies that retrieving pending payouts excludes non-pending ones.
    /// </summary>
    [Fact]
    public async Task GetPendingPayoutsAsync_ExcludesNonPendingPayouts()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Create a pending payout
        var voteId1 = await voteRepository.CreateVoteAsync(new VoteCreateDto
        {
            DuelId = 100500,
            AccountId = 2,
            ChosenOptionId = 100501,
            BetAmount = 10.00m,
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        var payoutDto = await repository.CreateAsync(new PayoutCreateDto
        {
            VoteId = voteId1,
            DuelId = 100500,
            AccountId = 1,
            Amount = 15.00m,
            ProcessedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Mark as completed
        await repository.UpdateAsync(new PayoutUpdateDto
        {
            Id = payoutDto.Id,
            Status = PayoutStatus.Completed
        }, CancellationToken.None);

        // Act
        var result = await repository.GetPendingPayoutsAsync(10, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that retrieving pending payouts respects the limit.
    /// </summary>
    [Fact]
    public async Task GetPendingPayoutsAsync_RespectsLimit()
    {
        // Arrange
        await _fixture.ResetAsync(CancellationToken.None);
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();
        var voteRepository = scope.ServiceProvider.GetRequiredService<IVoteRepository>();

        for (int i = 0; i < 5; i++)
        {
            var voteId = await voteRepository.CreateVoteAsync(new VoteCreateDto
            {
                DuelId = 100500,
                AccountId = 2,
                ChosenOptionId = 100501,
                BetAmount = 10.00m,
                CreatedAt = DateTimeOffset.UtcNow
            }, CancellationToken.None);

            await repository.CreateAsync(new PayoutCreateDto
            {
                VoteId = voteId,
                DuelId = 100500,
                AccountId = 1,
                Amount = 15.00m,
                ProcessedAt = DateTimeOffset.UtcNow
            }, CancellationToken.None);
        }

        // Act
        var result = await repository.GetPendingPayoutsAsync(3, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }

    /// <summary>
    /// Verifies that retrieving pending payouts when none exist returns empty list.
    /// </summary>
    [Fact]
    public async Task GetPendingPayoutsAsync_NoPendingPayouts_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IPayoutRepository>();

        // Act
        var result = await repository.GetPendingPayoutsAsync(10, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion
}
