using Mint.Database.Entities.Users.Sessions.Dto;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="UserSessionRepository"/>
/// </summary>
public class UserSessionRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public UserSessionRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    private readonly long _externalUserId1 = 1001;
    private readonly long _externalUserId2 = 1002;

    /// <summary>
    /// Verifies that creating a new session returns a valid session with generated ID.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateSessionAsync_NewSession_CreatesSuccessfully()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act
        var result = await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{\"key\": \"value\"}");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal(1, result.UserId);
        Assert.Equal(1, result.ScenarioId);
        Assert.Equal(1, result.CurrentStepId);
        Assert.Equal("{\"key\": \"value\"}", result.Data);
        Assert.Null(result.CompletedAt);
    }

    /// <summary>
    /// Verifies that creating a session with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateSessionAsync_NullData_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, null!));
    }

    /// <summary>
    /// Verifies that updating an existing session modifies the data correctly.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateSessionAsync_ExistingSession_UpdatesSuccessfully()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act
        var created = await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{\"initial\": \"data\"}");
        var result = await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 2, "{\"updated\": \"data\"}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal(2, result.CurrentStepId);
        Assert.Equal("{\"updated\": \"data\"}", result.Data);
    }

    /// <summary>
    /// Verifies that creating sessions for different users returns different IDs.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateSessionAsync_DifferentUsers_ReturnsDifferentIds()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act
        var result1 = await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{}");
        var result2 = await repository.CreateOrUpdateSessionAsync(_externalUserId2, 2, 2, "{}");

        // Assert
        Assert.NotEqual(result1.Id, result2.Id);
        Assert.True(result1.Id > 0);
        Assert.True(result2.Id > 0);
    }

    /// <summary>
    /// Verifies that getting an active session by user ID returns a valid session.
    /// </summary>
    [Fact]
    public async Task GetActiveSessionAsync_ExistingActiveSession_ReturnsSession()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{}");

        // Act
        var result = await repository.GetActiveSessionAsync(_externalUserId1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal(1, result.ScenarioId);
        Assert.Equal(1, result.CurrentStepId);
    }

    /// <summary>
    /// Verifies that getting an active session for a user without active session returns null.
    /// </summary>
    [Fact]
    public async Task GetActiveSessionAsync_NoActiveSession_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act
        var result = await repository.GetActiveSessionAsync(999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that getting a session by user ID and scenario ID returns the correct session.
    /// </summary>
    [Fact]
    public async Task GetSessionByUserIdAndScenarioAsync_ExistingSession_ReturnsSession()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{}");

        // Act
        var result = await repository.GetSessionByUserIdAndScenarioAsync(_externalUserId1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal(1, result.ScenarioId);
    }

    /// <summary>
    /// Verifies that getting a non-existent session returns null.
    /// </summary>
    [Fact]
    public async Task GetSessionByUserIdAndScenarioAsync_NonExistentSession_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act
        var result = await repository.GetSessionByUserIdAndScenarioAsync(999, 999);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that updating the current step changes it correctly.
    /// </summary>
    [Fact]
    public async Task UpdateCurrentStepAsync_ExistingSession_UpdatesStep()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        var created = await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{}");

        // Act
        var result = await repository.UpdateCurrentStepAsync(created.Id, 3);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.CurrentStepId);
    }

    /// <summary>
    /// Verifies that updating the current step for a non-existent session throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task UpdateCurrentStepAsync_NonExistentSession_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.UpdateCurrentStepAsync(999, 1));
    }

    /// <summary>
    /// Verifies that updating session data changes it correctly.
    /// </summary>
    [Fact]
    public async Task UpdateSessionDataAsync_ExistingSession_UpdatesData()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        var created = await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{}");

        // Act
        var result = await repository.UpdateSessionDataAsync(created.Id, "{\"newData\": \"newValue\"}");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("{\"newData\": \"newValue\"}", result.Data);
    }

    /// <summary>
    /// Verifies that updating session data for a non-existent session throws InvalidOperationException.
    /// </summary>
    [Fact]
    public async Task UpdateSessionDataAsync_NonExistentSession_ThrowsInvalidOperationException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await repository.UpdateSessionDataAsync(999, "{}"));
    }

    /// <summary>
    /// Verifies that completing a session marks it as completed.
    /// </summary>
    [Fact]
    public async Task CompleteSessionAsync_ExistingSession_MarksAsCompleted()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        var created = await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{}");

        // Act
        await repository.CompleteSessionAsync(created.Id);
        var result = await repository.GetActiveSessionAsync(_externalUserId1);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that completing a non-existent session does not throw.
    /// </summary>
    [Fact]
    public async Task CompleteSessionAsync_NonExistentSession_DoesNotThrow()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act
        await repository.CompleteSessionAsync(999);

        // Assert - no exception means success
        Assert.True(true);
    }

    /// <summary>
    /// Verifies that deleting a session removes it from the database.
    /// </summary>
    [Fact]
    public async Task DeleteSessionAsync_ExistingSession_DeletesSuccessfully()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        var created = await repository.CreateOrUpdateSessionAsync(_externalUserId1, 1, 1, "{}");

        // Act
        await repository.DeleteSessionAsync(created.Id);
        var result = await repository.GetActiveSessionAsync(1);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that deleting a non-existent session does not throw.
    /// </summary>
    [Fact]
    public async Task DeleteSessionAsync_NonExistentSession_DoesNotThrow()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IUserSessionRepository>();

        // Act
        await repository.DeleteSessionAsync(999);

        // Assert - no exception means success
        Assert.True(true);
    }
}
