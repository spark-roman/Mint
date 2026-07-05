using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Database;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Users;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.Ledger.Transactions.Dto;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="TransactionRepository"/>
/// </summary>
public class TransactionRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public TransactionRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that creating a transaction returns a valid transaction ID.
    /// </summary>
    [Fact]
    public async Task CreateTransactionAsync_CreatedTransaction_ReturnsTransactionId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var transaction = new TransactionCreateDto
        {
            CreditAccountId = 1,
            DebetAccountId = 1,
            Amount = 100.00m,
            Description = "Test transaction",
            CreatedAt = DateTimeOffset.UtcNow,
            BonusType = BonusType.Start
        };

        // Act
        var transactionId = await repository.CreateTransactionAsync(transaction, CancellationToken.None);

        // Assert
        Assert.True(transactionId > 0);
    }

    /// <summary>
    /// Verifies that creating a transaction decreases debit account balance and increases credit account balance.
    /// </summary>
    [Fact]
    public async Task CreateTransactionAsync_TransferBetweenAccounts_UpdatesBalancesCorrectly()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var amount = 500.00m;

        // Get initial balances
        var debitAccount = await accountRepository.GetAccountByIdAsync(1, CancellationToken.None);
        var creditAccount = await accountRepository.GetAccountByIdAsync(2, CancellationToken.None);
        Assert.NotNull(debitAccount);
        Assert.NotNull(creditAccount);

        var initialDebitBalance = debitAccount.Balance;
        var initialCreditBalance = creditAccount.Balance;

        var transaction = new TransactionCreateDto
        {
            CreditAccountId = 2,
            DebetAccountId = 1,
            Amount = amount,
            Description = "Transfer test",
            BonusType = BonusType.Start,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var transactionId = await repository.CreateTransactionAsync(transaction, CancellationToken.None);

        // Assert
        Assert.True(transactionId > 0);

        // Verify balances were updated
        var updatedDebitAccount = await accountRepository.GetAccountByIdAsync(1, CancellationToken.None);
        var updatedCreditAccount = await accountRepository.GetAccountByIdAsync(2, CancellationToken.None);

        Assert.NotNull(updatedDebitAccount);
        Assert.NotNull(updatedCreditAccount);

        Assert.Equal(initialDebitBalance - amount, updatedDebitAccount.Balance);
        Assert.Equal(initialCreditBalance + amount, updatedCreditAccount.Balance);
    }

    /// <summary>
    /// Verifies that creating a transaction with insufficient balance on debit account throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task CreateTransactionAsync_InsufficientDebitBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Get account 3 with balance 750.25m
        var debitAccount = await accountRepository.GetAccountByIdAsync(3, CancellationToken.None);
        Assert.NotNull(debitAccount);

        var amount = debitAccount.Balance + 1000.00m; // Amount exceeding balance

        var transaction = new TransactionCreateDto
        {
            CreditAccountId = 1,
            DebetAccountId = 3,
            Amount = amount,
            Description = "Insufficient balance test",
            BonusType = BonusType.Daily,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.CreateTransactionAsync(transaction, CancellationToken.None));

        Assert.Contains("Debet account balance is not enough", exception.Message);

        // Verify balance was not changed (transaction rolled back)
        var unchangedAccount = await accountRepository.GetAccountByIdAsync(3, CancellationToken.None);
        Assert.NotNull(unchangedAccount);
        Assert.Equal(debitAccount.Balance, unchangedAccount.Balance);
    }

    /// <summary>
    /// Verifies that creating a transaction with non-existent debit account throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task CreateTransactionAsync_NonExistentDebitAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        var transaction = new TransactionCreateDto
        {
            CreditAccountId = 1,
            DebetAccountId = 999999, // Non-existent account
            Amount = 100.00m,
            Description = "Non-existent debit account test",
            BonusType = BonusType.None,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.CreateTransactionAsync(transaction, CancellationToken.None));

        Assert.Contains("Debet account not found", exception.Message);
    }

    /// <summary>
    /// Verifies that creating a transaction with non-existent credit account throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task CreateTransactionAsync_NonExistentCreditAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        var transaction = new TransactionCreateDto
        {
            CreditAccountId = 999999, // Non-existent account
            DebetAccountId = 1,
            Amount = 100.00m,
            Description = "Non-existent credit account test",
            BonusType = BonusType.None,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.CreateTransactionAsync(transaction, CancellationToken.None));

        Assert.Contains("Credit account not found", exception.Message);
    }

    /// <summary>
    /// Verifies that creating a transaction with inactive debit account throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task CreateTransactionAsync_InactiveDebitAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        using var context = await _fixture.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>().CreateDbContextAsync();

        // Create a new inactive account for debit
        var inactiveAccount = new AccountEntity
        {
            Id = 100,
            UserId = 100,
            Balance = 10000.00m,
            Status = AccountStatus.Deleted,
            CreatedAt = DateTimeOffset.UtcNow,
            LastTransactionDate = DateTimeOffset.UtcNow
        };
        context.Accounts.Add(inactiveAccount);
        await context.SaveChangesAsync(CancellationToken.None);

        var transaction = new TransactionCreateDto
        {
            CreditAccountId = 2,
            DebetAccountId = 100,
            Amount = 100.00m,
            Description = "Inactive debit account test",
            BonusType = BonusType.None,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.CreateTransactionAsync(transaction, CancellationToken.None));

        Assert.Contains("Debet account is not active", exception.Message);
    }

    /// <summary>
    /// Verifies that creating a transaction with inactive credit account throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task CreateTransactionAsync_InactiveCreditAccount_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        using var context = await _fixture.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>().CreateDbContextAsync();

        // Create a new inactive account for credit
        var inactiveAccount = new AccountEntity
        {
            Id = 200,
            UserId = 200,
            Balance = 10000.00m,
            Status = AccountStatus.Deleted,
            CreatedAt = DateTimeOffset.UtcNow,
            LastTransactionDate = DateTimeOffset.UtcNow
        };
        context.Accounts.Add(inactiveAccount);
        await context.SaveChangesAsync(CancellationToken.None);

        var transaction = new TransactionCreateDto
        {
            CreditAccountId = 200,
            DebetAccountId = 1,
            Amount = 100.00m,
            Description = "Inactive credit account test",
            BonusType = BonusType.None,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => repository.CreateTransactionAsync(transaction, CancellationToken.None));

        Assert.Contains("Credit account is not active", exception.Message);
    }

    /// <summary>
    /// Verifies that retrieving a transaction by ID returns a valid TransactionDto.
    /// </summary>
    [Fact]
    public async Task GetTransactionByIdAsync_ExistingTransaction_ReturnsTransactionDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
        var transaction = new TransactionCreateDto
        {
            CreditAccountId = 1,
            DebetAccountId = 1,
            Amount = 250.50m,
            Description = "Click transaction",
            CreatedAt = DateTimeOffset.UtcNow,
            BonusType = BonusType.Start
        };

        // Act
        var transactionId = await repository.CreateTransactionAsync(transaction, CancellationToken.None);
        var result = await repository.GetTransactionByIdAsync(transactionId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.Id);
        Assert.Equal(1, result.DebetAccountId);
        Assert.Equal(250.50m, result.Amount);
        Assert.Equal(1, (int)result.BounusType);
        Assert.Equal("Click transaction", result.Description);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent transaction returns null.
    /// </summary>
    [Fact]
    public async Task GetTransactionByIdAsync_NonExistentTransaction_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        // Act
        var result = await repository.GetTransactionByIdAsync(0, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that retrieving transactions by account ID returns the correct list.
    /// </summary>
    [Fact]
    public async Task GetTransactionsByAccountIdAsync_ExistingAccount_ReturnsTransactionList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        await repository.CreateTransactionAsync(new TransactionCreateDto
        {
            CreditAccountId = 1,
            DebetAccountId = 1,
            Amount = 100.00m,
            Description = "First",
            CreatedAt = DateTimeOffset.UtcNow,
            BonusType = BonusType.Start
        }, CancellationToken.None);

        await repository.CreateTransactionAsync(new TransactionCreateDto
        {
            CreditAccountId = 1,
            DebetAccountId = 1,
            Amount = -50.00m,
            Description = "Second",
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        // Act
        var result = await repository.GetTransactionsByAccountIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(2, result.Count);
    }

    /// <summary>
    /// Verifies that retrieving transactions for an account without transactions returns empty list.
    /// </summary>
    [Fact]
    public async Task GetTransactionsByAccountIdAsync_AccountWithoutTransactions_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        // Act
        var result = await repository.GetTransactionsByAccountIdAsync(999999, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
