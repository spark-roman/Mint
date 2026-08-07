using AdvApplication.Auth.Users;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Duels.Handlers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
using Mint.UnitTests.AppServices.System.Duels.Fixtures;

namespace Mint.UnitTests.AppServices.System.Duels;

/// <summary>
/// Tests for <see cref="DuelHandler"/> using DI and EF Core.
/// </summary>
public class DuelHandlerTests : IClassFixture<DuelHandlerFixture>, IDisposable
{
    private readonly DuelHandlerFixture _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuelHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public DuelHandlerTests(DuelHandlerFixture fixture)
    {
        _fixture = fixture;
    }

    #region PlaceBetAsync - Successful Bet

    /// <summary>
    /// Verifies that PlaceBetAsync returns success for a valid bet.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_ReturnsSuccess()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act
        var result = await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Ставка успешно принята!", result.Message);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync creates a transaction record for a valid bet.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_CreatesTransaction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Act
        await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);

        // Assert
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(account.Id, CancellationToken.None);
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);
        Assert.Contains(transactions, t => t.Description?.Contains("дуэль") == true);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync creates a vote record for a valid bet.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_CreatesVote()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Act
        await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);

        // Assert
        var hasVoted = await voteRepository.HasUserVotedInDuelAsync(1001, 1, CancellationToken.None);
        Assert.True(hasVoted);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync links the transaction and vote via TransactionId.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_VoteReferencesTransaction()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Act
        var result = await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.VoteId > 0);

        var transaction = await transactionRepository.GetTransactionByIdAsync(result.TransactionId, CancellationToken.None);
        Assert.NotNull(transaction);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync returns the vote ID in the result.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_ReturnsVoteId()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act
        var result = await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.VoteId > 0);
    }

    #endregion

    #region PlaceBetAsync - Transaction Rollback on Vote Failure (Atomicity)

    /// <summary>
    /// Verifies that when transaction is created but vote creation fails,
    /// the transaction is rolled back (both are absent from the database).
    /// This tests the TransactionScope behavior: if scope.Complete() is not called,
    /// all changes within the scope are discarded.
    /// </summary>
    [Fact(Skip = "Ef inmemory does not support transactions")]
    public async Task PlaceBetAsync_VoteCreationFails_TransactionRolledBack()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();

        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var timeProvider = _currentScope.ServiceProvider.GetRequiredService<TimeProvider>();

        // Replace the vote repository with one that throws on CreateVoteAsync
        var failingVoteRepository = new FailingVoteRepository();
        var handler = new DuelHandler(
            duelRepository,
            accountRepository,
            userRepository,
            transactionRepository,
            failingVoteRepository,
            timeProvider);

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Act - PlaceBetAsync should throw due to failing vote repository
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None));

        // Assert - exception thrown
        Assert.NotNull(exception);

        // Verify that the transaction was also rolled back (not persisted)
        // The TransactionScope should have discarded the transaction when Complete() was not called
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(account!.Id, CancellationToken.None);
        Assert.Empty(transactions ?? []);
    }

    /// <summary>
    /// Verifies that when vote creation fails, no vote record is created in the database.
    /// </summary>
    [Fact(Skip = "Ef inmemory does not support transactions")]
    public async Task PlaceBetAsync_VoteCreationFails_NoVoteCreated()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();

        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var timeProvider = _currentScope.ServiceProvider.GetRequiredService<TimeProvider>();

        var failingVoteRepository = new FailingVoteRepository();
        var handler = new DuelHandler(
            duelRepository,
            accountRepository,
            userRepository,
            transactionRepository,
            failingVoteRepository,
            timeProvider);

        // Act
        try
        {
            await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            // Expected to throw
        }

        // Assert - no vote should exist
        var votes = await voteRepository.GetVotesByDuelIdAsync(1, CancellationToken.None);
        Assert.Empty(votes ?? []);
    }

    /// <summary>
    /// Verifies that when vote creation fails, the user's balance is not decreased
    /// (transaction is fully rolled back).
    /// </summary>
    [Fact(Skip = "Ef inmemory does not support transactions")]
    public async Task PlaceBetAsync_VoteCreationFails_BalanceUnchanged()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();

        var duelRepository = _currentScope.ServiceProvider.GetRequiredService<IDuelRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var userRepository = _currentScope.ServiceProvider.GetRequiredService<IUserRepository>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var timeProvider = _currentScope.ServiceProvider.GetRequiredService<TimeProvider>();

        var initialAccount = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(initialAccount);
        var initialBalance = initialAccount!.Balance;

        var failingVoteRepository = new FailingVoteRepository();
        var handler = new DuelHandler(
            duelRepository,
            accountRepository,
            userRepository,
            transactionRepository,
            failingVoteRepository,
            timeProvider);

        // Act
        try
        {
            await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            // Expected to throw
        }

        // Assert - balance should be unchanged
        var newAccount = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(newAccount);
        Assert.Equal(initialBalance, newAccount.Balance);
    }

    #endregion

    #region PlaceBetAsync - Race Condition

    /// <summary>
    /// Verifies that when the same user tries to place two bets on the same duel,
    /// the second bet is rejected (user already voted check).
    /// This simulates a race condition where two requests arrive for the same user/duel.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_SecondBetSameUserSameDuel_Rejected()
    {
        // Arrange - First bet
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler1 = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // First bet succeeds
        var firstResult = await handler1.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);
        Assert.True(firstResult.Success);

        // Arrange - Second bet in a new scope (simulating a concurrent request)
        _currentScope.Dispose();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler2 = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act - Second bet by same user on same duel
        var secondResult = await handler2.PlaceBetAsync(1001, 1, 1, 200m, CancellationToken.None);

        // Assert - second bet should be rejected
        Assert.False(secondResult.Success);
        Assert.Contains("уже сделали ставку", secondResult.Message);
    }

    /// <summary>
    /// Verifies that different users can place bets on the same duel independently.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_DifferentUsersSameDuel_AllSucceed()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Act - User 1001 bets
        var result1 = await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);

        // Act - User 1002 bets on same duel
        var result2 = await handler.PlaceBetAsync(1002, 1, 2, 200m, CancellationToken.None);

        // Assert
        Assert.True(result1.Success);
        Assert.True(result2.Success);

        // Verify both votes exist
        var hasVoted1 = await voteRepository.HasUserVotedInDuelAsync(1001, 1, CancellationToken.None);
        var hasVoted2 = await voteRepository.HasUserVotedInDuelAsync(1002, 1, CancellationToken.None);
        Assert.True(hasVoted1);
        Assert.True(hasVoted2);
    }

    /// <summary>
    /// Verifies that user 1001 can bet on duel 1 and then on duel 2 (different duels).
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_SameUserDifferentDuels_AllSucceed()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Act
        var result1 = await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);
        var result2 = await handler.PlaceBetAsync(1001, 2, 3, 150m, CancellationToken.None);

        // Assert
        Assert.True(result1.Success);
        Assert.True(result2.Success);

        var hasVotedDuel1 = await voteRepository.HasUserVotedInDuelAsync(1001, 1, CancellationToken.None);
        var hasVotedDuel2 = await voteRepository.HasUserVotedInDuelAsync(1001, 2, CancellationToken.None);
        Assert.True(hasVotedDuel1);
        Assert.True(hasVotedDuel2);
    }

    #endregion

    #region PlaceBetAsync - Error Cases

    /// <summary>
    /// Verifies that PlaceBetAsync returns failure when duel is not found.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_DuelNotFound_ReturnsFailure()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act
        var result = await handler.PlaceBetAsync(1001, 999, 1, 100m, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Дуэль не найдена", result.Message);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync returns failure when duel is expired.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ExpiredDuel_ReturnsFailure()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act
        var result = await handler.PlaceBetAsync(1001, 3, 5, 100m, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("дуэль уже закрыта", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync returns failure when user has already voted.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_UserAlreadyVoted_ReturnsFailure()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Pre-create a vote
        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        var voteRepo = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var vote = new VoteCreateDto
        {
            DuelId = 1,
            ChosenOptionId = 1,
            AccountId = account.Id,
            BetAmount = 50m,
            TransactionId = 999,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await voteRepo.CreateVoteAsync(vote, CancellationToken.None);

        // Act
        var result = await handler.PlaceBetAsync(1001, 1, 2, 100m, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("уже сделали ставку", result.Message);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync returns failure when option is not found.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_OptionNotFound_ReturnsFailure()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act
        var result = await handler.PlaceBetAsync(1001, 1, 999, 100m, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Вариант ответа не найден", result.Message);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync returns failure when user is not found.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_UserNotFound_ReturnsFailure()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act
        var result = await handler.PlaceBetAsync(99999, 1, 1, 100m, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Пользователь не найден", result.Message);
    }

    /// <summary>
    /// Verifies that PlaceBetAsync returns failure when balance is insufficient.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_InsufficientBalance_ReturnsFailure()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act
        var result = await handler.PlaceBetAsync(1001, 1, 1, 999999999m, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Contains("Недостаточно средств", result.Message);
    }

    #endregion

    #region GetFirstAvailableDuelAsync

    /// <summary>
    /// Verifies that GetFirstAvailableDuelAsync returns a duel for a user who hasn't voted.
    /// </summary>
    [Fact]
    public async Task GetFirstAvailableDuelAsync_UserNotVoted_ReturnsDuel()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Act
        var result = await handler.GetFirstAvailableDuelAsync(3, account.Id, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.DuelId);
        Assert.Contains("Bitcoin", result.Question);
    }

    /// <summary>
    /// Verifies that GetFirstAvailableDuelAsync returns null when user has already voted.
    /// </summary>
    [Fact]
    public async Task GetFirstAvailableDuelAsync_UserAlreadyVoted_ReturnsNull()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Pre-create a vote
        var vote = new VoteCreateDto
        {
            DuelId = 1,
            ChosenOptionId = 1,
            AccountId = account.Id,
            BetAmount = 100m,
            TransactionId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await voteRepository.CreateVoteAsync(vote, CancellationToken.None);

        // Act
        var result = await handler.GetFirstAvailableDuelAsync(1, account.Id, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that GetFirstAvailableDuelAsync returns null when no duels exist in category.
    /// </summary>
    [Fact]
    public async Task GetFirstAvailableDuelAsync_NoDuelsInCategory_ReturnsNull()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Act
        var result = await handler.GetFirstAvailableDuelAsync(999, account.Id, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    #endregion

    #region HasUserVotedInDuelAsync

    /// <summary>
    /// Verifies that HasUserVotedInDuelAsync returns false when user has not voted.
    /// </summary>
    [Fact]
    public async Task HasUserVotedInDuelAsync_UserNotVoted_ReturnsFalse()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // Act
        var result = await handler.HasUserVotedInDuelAsync(1001, 1, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that HasUserVotedInDuelAsync returns true when user has voted.
    /// </summary>
    [Fact]
    public async Task HasUserVotedInDuelAsync_UserVoted_ReturnsTrue()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        var vote = new VoteCreateDto
        {
            DuelId = 1,
            ChosenOptionId = 1,
            AccountId = account.Id,
            BetAmount = 100m,
            TransactionId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await voteRepository.CreateVoteAsync(vote, CancellationToken.None);

        // Act
        var result = await handler.HasUserVotedInDuelAsync(1001, 1, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that HasUserVotedInDuelAsync returns false for different duel.
    /// </summary>
    [Fact]
    public async Task HasUserVotedInDuelAsync_VotedInDifferentDuel_ReturnsFalse()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        var vote = new VoteCreateDto
        {
            DuelId = 1,
            ChosenOptionId = 1,
            AccountId = account.Id,
            BetAmount = 100m,
            TransactionId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };
        await voteRepository.CreateVoteAsync(vote, CancellationToken.None);

        // Act
        var result = await handler.HasUserVotedInDuelAsync(1001, 2, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Transaction and Vote Consistency

    /// <summary>
    /// Verifies that transaction amount matches the bet amount.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_TransactionAmountMatchesBet()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Act
        await handler.PlaceBetAsync(1001, 1, 1, 250m, CancellationToken.None);

        // Assert
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(account.Id, CancellationToken.None);
        var betTransaction = transactions!.FirstOrDefault(t => t.Description?.Contains("дуэль") == true);
        Assert.NotNull(betTransaction);
        Assert.Equal(250m, betTransaction.Amount);
    }

    /// <summary>
    /// Verifies that bet transaction description contains duel ID.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_TransactionDescriptionContainsDuelId()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Act
        await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);

        // Assert
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(account.Id, CancellationToken.None);
        var betTransaction = transactions!.FirstOrDefault(t => t.Description?.Contains("дуэль") == true);
        Assert.NotNull(betTransaction);
        Assert.Contains("дуэль", betTransaction.Description!, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("1", betTransaction.Description);
    }

    /// <summary>
    /// Verifies that vote has correct duel ID and option ID.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_VoteHasCorrectDuelAndOption()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        // Act
        await handler.PlaceBetAsync(1001, 1, 2, 100m, CancellationToken.None);

        // Assert
        var vote = await voteRepository.GetVoteAsync(1, account.Id, CancellationToken.None);
        Assert.NotNull(vote);
        Assert.Equal(1, vote.DuelId);
        Assert.Equal(2, vote.ChosenOptionId);
        Assert.Equal(100m, vote.BetAmount);
    }

    /// <summary>
    /// Verifies that user balance is correctly decreased after a successful bet.
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_ValidBet_BalanceDecreasedByBetAmount()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);
        var initialBalance = account.Balance;

        // Act
        await handler.PlaceBetAsync(1001, 1, 1, 500m, CancellationToken.None);

        // Assert
        var newBalance = await accountRepository.GetUserBalanceAsync(1001, CancellationToken.None);
        Assert.Equal(initialBalance - 500m, newBalance);
    }

    #endregion

    #region Multiple Bets on Same Option

    /// <summary>
    /// Verifies that user can place multiple bets on the same option of the same duel
    /// (only one vote per user per duel is allowed, subsequent bets should fail).
    /// </summary>
    [Fact]
    public async Task PlaceBetAsync_MultipleBetsSameOption_SecondRejected()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredService<IDuelHandler>();

        // First bet
        var result1 = await handler.PlaceBetAsync(1001, 1, 1, 100m, CancellationToken.None);
        Assert.True(result1.Success);

        // Second bet on same option
        var result2 = await handler.PlaceBetAsync(1001, 1, 1, 200m, CancellationToken.None);

        // Assert
        Assert.False(result2.Success);
        Assert.Contains("уже сделали ставку", result2.Message);
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

    /// <summary>
    /// Vote repository that always throws on CreateVoteAsync to simulate vote creation failure.
    /// </summary>
    private sealed class FailingVoteRepository : IVoteRepository
    {
        /// <inheritdoc />
        public Task<long> CreateVoteAsync(VoteCreateDto dto, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException("Vote creation failed (simulated)");
        }

        /// <inheritdoc />
        public Task<VoteDto?> GetVoteAsync(long duelId, long accountId, CancellationToken cancellationToken)
        {
            return Task.FromResult<VoteDto?>(null);
        }

        /// <inheritdoc />
        public Task<List<VoteDto>?> GetVotesByDuelIdAsync(long duelId, CancellationToken cancellationToken)
        {
            return Task.FromResult<List<VoteDto>?>([]);
        }

        /// <inheritdoc />
        public Task<VoteEntity?> GetVoteByDuelAndAccountAsync(long duelId, long accountId, CancellationToken cancellationToken)
        {
            return Task.FromResult<VoteEntity?>(null);
        }

        /// <inheritdoc />
        public Task<bool> HasAccountVotedAsync(long duelId, long accountId, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> HasUserVotedInDuelAsync(long externalUserId, long duelId, CancellationToken cancellationToken)
        {
            return Task.FromResult(false);
        }
    }
}
