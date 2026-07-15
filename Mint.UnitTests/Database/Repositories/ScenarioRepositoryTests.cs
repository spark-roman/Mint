using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="ScenarioRepository"/>
/// </summary>
public class ScenarioRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public ScenarioRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that retrieving a scenario by name returns a valid ScenarioDto.
    /// </summary>
    [Fact]
    public async Task GetScenarioByNameAsync_ExistingScenario_ReturnsScenarioDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetScenarioByNameAsync("start", _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("start", result.Name);
        Assert.True(result.IsActive);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent scenario by name returns null.
    /// </summary>
    [Fact]
    public async Task GetScenarioByNameAsync_NonExistentScenario_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetScenarioByNameAsync("NonExistentScenario", _cancellationToken);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that retrieving a scenario by ID returns a valid ScenarioDto.
    /// </summary>
    [Fact]
    public async Task GetScenarioByIdAsync_ExistingScenario_ReturnsScenarioDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetScenarioByIdAsync(1, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("start", result.Name);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent scenario by ID returns null.
    /// </summary>
    [Fact]
    public async Task GetScenarioByIdAsync_NonExistentScenario_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetScenarioByIdAsync(999, _cancellationToken);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that getting all scenarios returns the correct count.
    /// </summary>
    [Fact]
    public async Task GetAllScenariosAsync_ReturnsAllScenarios()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetAllScenariosAsync(_cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count);
    }

    /// <summary>
    /// Verifies that retrieving a step by ID returns a valid StepDto.
    /// </summary>
    [Fact]
    public async Task GetStepByIdAsync_ExistingStep_ReturnsStepDto()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetStepByIdAsync(1, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Добро пожаловать", result.Message);
    }

    /// <summary>
    /// Verifies that retrieving a non-existent step by ID returns null.
    /// </summary>
    [Fact]
    public async Task GetStepByIdAsync_NonExistentStep_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetStepByIdAsync(999, _cancellationToken);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that getting the first step of a scenario returns the correct step.
    /// </summary>
    [Fact]
    public async Task GetFirstStepByScenarioIdAsync_ReturnsFirstStep()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetFirstStepByScenarioIdAsync(1, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.OrderNum);
    }

    /// <summary>
    /// Verifies that getting steps by scenario ID returns all steps ordered.
    /// </summary>
    [Fact]
    public async Task GetStepsByScenarioIdAsync_ReturnsOrderedSteps()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetStepsByScenarioIdAsync(1, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(1, result[0].OrderNum);
    }

    /// <summary>
    /// Verifies that getting buttons by step ID returns all buttons ordered.
    /// </summary>
    [Fact]
    public async Task GetButtonsByStepIdAsync_ReturnsOrderedButtons()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetButtonsByStepIdAsync(1, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result[0].OrderNum);
        Assert.Equal(2, result[1].OrderNum);
        Assert.Equal(3, result[2].OrderNum);
    }

    /// <summary>
    /// Verifies that getting a button by action returns the correct button.
    /// </summary>
    [Fact]
    public async Task GetButtonByActionAsync_ExistingAction_ReturnsButton()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetButtonByActionAsync("duels", _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("📊 Дуэли дня", result.Caption);
        Assert.Equal("duels", result.Action);
    }

    /// <summary>
    /// Verifies that getting a button by non-existent action returns null.
    /// </summary>
    [Fact]
    public async Task GetButtonByActionAsync_NonExistentAction_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetButtonByActionAsync("nonexistent", _cancellationToken);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that getting a button by ID returns the correct button.
    /// </summary>
    [Fact]
    public async Task GetButtonByIdAsync_ExistingButton_ReturnsButton()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetButtonByIdAsync(1, _cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("📊 Дуэли дня", result.Caption);
    }

    /// <summary>
    /// Verifies that getting a non-existent button by ID returns null.
    /// </summary>
    [Fact]
    public async Task GetButtonByIdAsync_NonExistentButton_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetButtonByIdAsync(999, _cancellationToken);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that getting the next step by button ID returns null (no next step configured).
    /// </summary>
    [Fact]
    public async Task GetNextStepByButtonIdAsync_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        // Act
        var result = await repository.GetNextStepByButtonIdAsync(1, _cancellationToken);

        // Assert
        Assert.Null(result);
    }
}