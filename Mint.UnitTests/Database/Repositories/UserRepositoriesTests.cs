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
}