using Mint.Common.Contracts.Transactions;
using Mint.Database.Entities.Transactions.Dto;
using Mint.Database.Entities.Transactions.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

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
            AccountId = 1,
            Amount = 100.00m,
            TransactionType = TransactionType.Bonus,
            Description = "Test transaction",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var transactionId = await repository.CreateTransactionAsync(transaction, CancellationToken.None);

        // Assert
        Assert.True(transactionId > 0);
    }

    /// <summary>
    /// Verifies that creating a transaction with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateTransactionAsync_NullTransaction_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateTransactionAsync(null!, CancellationToken.None));
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
            AccountId = 1,
            Amount = 250.50m,
            TransactionType = TransactionType.Click,
            Description = "Click transaction",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var transactionId = await repository.CreateTransactionAsync(transaction, CancellationToken.None);
        var result = await repository.GetTransactionByIdAsync(transactionId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.Id);
        Assert.Equal(1, result.AccountId);
        Assert.Equal(250.50m, result.Amount);
        Assert.Equal(TransactionType.Click, result.TransactionType);
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
            AccountId = 1,
            Amount = 100.00m,
            TransactionType = TransactionType.Bonus,
            Description = "First",
            CreatedAt = DateTimeOffset.UtcNow
        }, CancellationToken.None);

        await repository.CreateTransactionAsync(new TransactionCreateDto
        {
            AccountId = 1,
            Amount = -50.00m,
            TransactionType = TransactionType.Spend,
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
