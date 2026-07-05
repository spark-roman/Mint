using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.Ledger.Accounts.Dto;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Accounts.Repositories;
using Mint.Common.Contracts.Ledger.Accounts;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="AccountRepository"/>
/// </summary>
public class AccountRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public AccountRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that creating an account for an existing user returns a valid account ID.
    /// </summary>
    [Fact]
    public async Task CreateAccountAsync_CreatedAccount_ReturnsAccountId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var account = new AccountCreateDto
        {
            ExternalUserId = 1001, // Existing user in seed data
            SystemType = 1,
            Balance = 500.00m,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var accountId = await repository.CreateAccountAsync(account, CancellationToken.None);

        // Assert
        Assert.True(accountId > 0);
    }

    /// <summary>
    /// Verifies that creating an account for a non-existent user throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task CreateAccountAsync_NonExistentUser_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var account = new AccountCreateDto
        {
            ExternalUserId = 999999, // Non-existent user
            SystemType = 1,
            Balance = 500.00m,
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.CreateAccountAsync(account, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that retrieving an existing account by ID returns a valid AccountDto with correct data.
    /// </summary>
    [Fact]
    public async Task GetAccountByIdAsync_ExistingAccount_ReturnsAccountDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act
        var result = await repository.GetAccountByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.UserId);
        Assert.Equal(1500.50m, result.Balance);
        Assert.Equal(AccountStatus.Active, result.Status);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent account returns null.
    /// </summary>
    [Fact]
    public async Task GetAccountByIdAsync_NonExistentAccount_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act
        var result = await repository.GetAccountByIdAsync(0, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that retrieving an account by external user ID and system type returns the correct account.
    /// </summary>
    [Fact]
    public async Task GetAccountByExternalUserIdAsync_ExistingUser_ReturnsAccount()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act
        var result = await repository.GetAccountByExternalUserIdAsync(1002, 1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.UserId);
        Assert.Equal(3200.00m, result.Balance);
        Assert.Equal(AccountStatus.Active, result.Status);
    }

    /// <summary>
    /// Verifies that retrieving an account for a user without accounts returns null.
    /// </summary>
    [Fact]
    public async Task GetAccountByExternalUserIdAsync_UserWithoutAccounts_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act
        var result = await repository.GetAccountByExternalUserIdAsync(999999, 1, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that updating the balance of an existing account returns true and updates the data.
    /// </summary>
    [Fact]
    public async Task UpdateBalanceAsync_ExistingAccount_UpdatesBalance_ReturnsTrue()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var dto = new AccountUpdateBalanceDto
        {
            AccountId = 1,
            NewBalance = 2000.00m,
            LastTransactionDate = new DateTimeOffset(2024, 8, 1, 10, 0, 0, TimeSpan.Zero)
        };

        // Act
        var result = await repository.UpdateBalanceAsync(dto, CancellationToken.None);

        // Assert
        Assert.True(result);

        // Verify the update
        var updatedAccount = await repository.GetAccountByIdAsync(1, CancellationToken.None);
        Assert.NotNull(updatedAccount);
        Assert.Equal(dto.NewBalance, updatedAccount.Balance);
        Assert.Equal(dto.LastTransactionDate, updatedAccount.LastTransactionDate);
    }

    /// <summary>
    /// Verifies that updating the balance of a non-existent account returns false.
    /// </summary>
    [Fact]
    public async Task UpdateBalanceAsync_NonExistentAccount_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
        var dto = new AccountUpdateBalanceDto
        {
            AccountId = 0,
            NewBalance = 100.00m,
            LastTransactionDate = DateTimeOffset.UtcNow
        };

        // Act
        var result = await repository.UpdateBalanceAsync(dto, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that passing null to UpdateBalanceAsync throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task UpdateBalanceAsync_NullDto_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.UpdateBalanceAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that passing null to CreateAccountAsync throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateAccountAsync_NullAccount_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateAccountAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that deleting an existing account returns true and sets status to Deleted.
    /// </summary>
    [Fact]
    public async Task DeleteAccountAsync_ExistingAccount_DeletesAccount_ReturnsTrue()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act
        var result = await repository.DeleteAccountAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result);

        // Verify the deletion
        var deletedAccount = await repository.GetAccountByIdAsync(1, CancellationToken.None);
        Assert.NotNull(deletedAccount);
        Assert.Equal(AccountStatus.Deleted, deletedAccount.Status);
    }

    /// <summary>
    /// Verifies that deleting a non-existent account returns false.
    /// </summary>
    [Fact]
    public async Task DeleteAccountAsync_NonExistentAccount_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act
        var result = await repository.DeleteAccountAsync(999999, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
