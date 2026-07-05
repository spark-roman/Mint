using AdvApplication.Auth.Users;
using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Users.Dto;
using Mint.Database.Entities.Users.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="UserRepository"/>
/// </summary>
public class UserRepositoriesTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    public UserRepositoriesTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);

        _fixture = fixture;
    }
    
    /// <summary>
    /// Verifies that creating a user returns a valid user ID.
    /// </summary>
    [Fact]
    public async Task CreateUserAsync_CreatedUser_ReturnsUserId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var user = new UserCreateDto
        {
            ExternalUserId = 99999,
            SystemType = 1,
            FirstName = "Test",
            LastName = "User",
            UserName = "test.user",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var userId = await repository.CreateUserAsync(user, CancellationToken.None);

        // Assert
        Assert.True(userId > 0);
    }

    /// <summary>
    /// Verifies that retrieving an existing user returns a valid UserDto with correct data.
    /// </summary>
    [Fact]
    public async Task GetUserAsync_ExistingUser_ReturnsUserDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        // Act
        var result = await repository.GetUserAsync(1001, 1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1001, result.ExternalUserId);
        Assert.Equal(1, result.SystemType);
        Assert.Equal("Alice", result.FirstName);
        Assert.Equal("Smith", result.LastName);
        Assert.Equal("alice.smith", result.UserName);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent user returns null.
    /// </summary>
    [Fact]
    public async Task GetUserAsync_NonExistentUser_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        // Act
        var result = await repository.GetUserAsync(0, 0, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that changing the status of an existing user returns true.
    /// </summary>
    [Fact]
    public async Task ChangeUserStatusAsync_ExistingUser_ChangesStatus_ReturnsTrue()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        // Act
        var result = await repository.ChangeUserStatusAsync(1001, 1, UserStatus.Blocked, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that changing the status of a non-existent user returns false.
    /// </summary>
    [Fact]
    public async Task ChangeUserStatusAsync_NonExistentUser_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        // Act
        var result = await repository.ChangeUserStatusAsync(0, 0, UserStatus.Blocked, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that passing null to CreateUserAsync throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateUserAsync_NullUser_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateUserAsync(null!, CancellationToken.None));
    }

    #region CreateOrUpdateUserAsync - Happy Path

    /// <summary>
    /// Verifies that CreateOrUpdateUserAsync creates a new user when it does not exist.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateUserAsync_NewUser_CreatesUserAndReturnsId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var user = new UserCreateDto
        {
            ExternalUserId = 88888,
            SystemType = 1,
            FirstName = "New",
            LastName = "User",
            UserName = "new_user",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var userId = await repository.CreateOrUpdateUserAsync(user, CancellationToken.None);

        // Assert
        Assert.True(userId > 0);

        // Verify user was created
        var createdUser = await repository.GetUserAsync(88888, 1, CancellationToken.None);
        Assert.NotNull(createdUser);
        Assert.Equal("New", createdUser.FirstName);
        Assert.Equal("User", createdUser.LastName);
        Assert.Equal("new_user", createdUser.UserName);
    }

    /// <summary>
    /// Verifies that CreateOrUpdateUserAsync updates an existing user's fields and LastAuthDate.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateUserAsync_ExistingUser_UpdatesFieldsAndLastAuthDate()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var existingUser = await repository.GetUserAsync(1001, 1, CancellationToken.None);
        Assert.NotNull(existingUser);
        var originalLastName = existingUser.LastName;

        var updateDto = new UserCreateDto
        {
            ExternalUserId = 1001,
            SystemType = 1,
            FirstName = "Alice",
            LastName = "Updated",
            UserName = "alice.updated",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var userId = await repository.CreateOrUpdateUserAsync(updateDto, CancellationToken.None);

        // Assert
        Assert.Equal(existingUser.Id, userId);

        var updatedUser = await repository.GetUserAsync(1001, 1, CancellationToken.None);
        Assert.NotNull(updatedUser);
        Assert.Equal("Alice", updatedUser.FirstName);
        Assert.Equal("Updated", updatedUser.LastName);
        Assert.Equal("alice.updated", updatedUser.UserName);
        Assert.NotNull(updatedUser.LastAuthDate);
    }

    /// <summary>
    /// Verifies that CreateOrUpdateUserAsync returns the same ID for an existing user.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateUserAsync_ExistingUser_ReturnsExistingUserId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var existingUser = await repository.GetUserAsync(1002, 1, CancellationToken.None);
        Assert.NotNull(existingUser);

        var updateDto = new UserCreateDto
        {
            ExternalUserId = 1002,
            SystemType = 1,
            FirstName = "Bob",
            LastName = "Johnson",
            UserName = "bob.johnson",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var userId = await repository.CreateOrUpdateUserAsync(updateDto, CancellationToken.None);

        // Assert
        Assert.Equal(existingUser.Id, userId);
    }

    /// <summary>
    /// Verifies that CreateOrUpdateUserAsync updates LastAuthDate to current time.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateUserAsync_ExistingUser_UpdatesLastAuthDateToCurrentTime()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var beforeUpdate = await repository.GetUserAsync(1001, 1, CancellationToken.None);
        var originalLastAuth = beforeUpdate?.LastAuthDate;

        var updateDto = new UserCreateDto
        {
            ExternalUserId = 1001,
            SystemType = 1,
            FirstName = "Alice",
            LastName = "Smith",
            UserName = "alice.smith",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await Task.Delay(10); // Ensure time difference

        // Act
        await repository.CreateOrUpdateUserAsync(updateDto, CancellationToken.None);

        // Assert
        var afterUpdate = await repository.GetUserAsync(1001, 1, CancellationToken.None);
        Assert.NotNull(afterUpdate);
        Assert.NotNull(afterUpdate.LastAuthDate);
        if (originalLastAuth.HasValue)
        {
            Assert.True(afterUpdate.LastAuthDate.Value > originalLastAuth.Value);
        }
    }

    #endregion

    #region CreateOrUpdateUserAsync - Edge Cases

    /// <summary>
    /// Verifies that CreateOrUpdateUserAsync throws ArgumentNullException when user is null.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateUserAsync_NullUser_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateOrUpdateUserAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that CreateOrUpdateUserAsync with different system type creates a new user.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateUserAsync_DifferentSystemType_CreatesNewUser()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        // User 1001 with SystemType=1 exists in seed data
        var userTg = new UserCreateDto
        {
            ExternalUserId = 1001,
            SystemType = 2, // Different system type
            FirstName = "Alice",
            LastName = "Web",
            UserName = "alice.web",
            CreatedAt = DateTimeOffset.UtcNow
        };

        // Act
        var userId = await repository.CreateOrUpdateUserAsync(userTg, CancellationToken.None);

        // Assert
        Assert.True(userId > 0);

        var createdUser = await repository.GetUserAsync(1001, 2, CancellationToken.None);
        Assert.NotNull(createdUser);
        Assert.Equal("Web", createdUser.LastName);
    }

    #endregion
}