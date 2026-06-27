using Mint.Database;
using Mint.Database.Entities.System.Dto;
using Mint.Database.Entities.System.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.UserCategories;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="AiPromptRepository"/>
/// </summary>
public class AiPromptRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public AiPromptRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that creating a prompt returns a valid ID.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateAsync_NewPrompt_ReturnsId()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        var prompt = new AiPromptCreateDto
        {
            UserPromptTemplate = "User prompt template",
            SystemPromptTemplate = "System prompt template",
            Temperature = 0.7f,
            MaxDuelsPerRun = 5
        };

        // Act
        var id = await repository.CreateOrUpdateAsync(prompt, CancellationToken.None);

        // Assert
        Assert.True(id > 0);
    }

    /// <summary>
    /// Verifies that creating a prompt with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateAsync_NullPrompt_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateOrUpdateAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that retrieving a prompt returns the correct data.
    /// </summary>
    [Fact]
    public async Task GetPromptsAsync_ExistingPrompt_ReturnsPrompt()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        var prompt = new AiPromptCreateDto
        {
            UserPromptTemplate = "User prompt template",
            SystemPromptTemplate = "Core prompt text",
            Temperature = 0.8f,
            MaxDuelsPerRun = 10
        };

        await repository.CreateOrUpdateAsync(prompt, CancellationToken.None);

        // Act
        var result = await repository.GetPromptsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Core prompt text", result.First().SystemPromptTemplate);
        Assert.Equal(0.8f, result.First().Temperature);
        Assert.Equal(10, result.First().MaxDuelsPerRun);
    }

    /// <summary>
    /// Verifies that retrieving a prompt when none exist returns empty list.
    /// </summary>
    [Fact]
    public async Task GetPromptsAsync_NoPrompt_ReturnsEmptyList()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();

        // Clear any existing prompts
        using var scope2 = _fixture.ServiceProvider.CreateScope();
        var dbContextFactory = scope2.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        context.AiPrompts.RemoveRange(context.AiPrompts);
        await context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetPromptsAsync(CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }

    /// <summary>
    /// Verifies that updating a prompt modifies existing data.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateAsync_UpdatePrompt_ModifiesData()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        
        var initialPrompt = new AiPromptCreateDto
        {
            SystemPromptTemplate = "Initial text",
            UserPromptTemplate = "Initial user prompt",
            Temperature = 0.5f,
            MaxDuelsPerRun = 3
        };

        await repository.CreateOrUpdateAsync(initialPrompt, CancellationToken.None);

        var updatedPrompt = new AiPromptCreateDto
        {
            SystemPromptTemplate = "Updated text",
            UserPromptTemplate = "Updated user prompt",
            Temperature = 0.9f,
            MaxDuelsPerRun = 15
        };

        // Act
        await repository.CreateOrUpdateAsync(updatedPrompt, CancellationToken.None);
        var result = await repository.GetPromptsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Updated text", result.First().SystemPromptTemplate);
        Assert.Equal(0.9f, result.First().Temperature);
        Assert.Equal(15, result.First().MaxDuelsPerRun);
    }

    /// <summary>
    /// Verifies that default values are applied when not specified.
    /// </summary>
    [Fact]
    public async Task CreateOrUpdateAsync_DefaultValues_AppliedCorrectly()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        var prompt = new AiPromptCreateDto
        {
            SystemPromptTemplate = "Test prompt",
            UserPromptTemplate = "Test user prompt"
        };

        // Act
        await repository.CreateOrUpdateAsync(prompt, CancellationToken.None);
        var result = await repository.GetPromptsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(0.6f, result.First().Temperature);
        Assert.Equal(3, result.First().MaxDuelsPerRun);
    }

    /// <summary>
    /// Verifies that retrieving a prompt returns active categories.
    /// </summary>
    [Fact]
    public async Task GetPromptsAsync_WithCategories_ReturnsActiveCategories()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        
        var prompt = new AiPromptCreateDto
        {
            SystemPromptTemplate = "Test prompt",
            UserPromptTemplate = "Test user prompt",
            Temperature = 0.7f,
            MaxDuelsPerRun = 5
        };

        await repository.CreateOrUpdateAsync(prompt, CancellationToken.None);

        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        var aiPrompt = await context.AiPrompts.FirstOrDefaultAsync();
        
        if (aiPrompt is not null)
        {
            aiPrompt.Categories.Add(new CategoryEntity
            {
                Name = "Crypto",
                IsActiveForAI = true,
                SearchKeywords = "Bitcoin, Ethereum"
            });
            aiPrompt.Categories.Add(new CategoryEntity
            {
                Name = "Inactive",
                IsActiveForAI = false,
                SearchKeywords = "test"
            });
            await context.SaveChangesAsync(CancellationToken.None);
        }

        // Act
        var result = await repository.GetPromptsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Single(result.First().Categories);
        Assert.Equal("Crypto", result.First().Categories.First().Name);
    }

    /// <summary>
    /// Verifies that retrieving a prompt returns empty categories when none exist.
    /// </summary>
    [Fact]
    public async Task GetPromptsAsync_WithoutCategories_ReturnsEmptyCategories()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IAiPromptRepository>();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<MintDbContext>>();
        
        // Clear existing data
        using var context = await dbContextFactory.CreateDbContextAsync(CancellationToken.None);
        context.AiPrompts.RemoveRange(context.AiPrompts);
        await context.SaveChangesAsync(CancellationToken.None);
        
        var prompt = new AiPromptCreateDto
        {
            SystemPromptTemplate = "Test prompt",
            UserPromptTemplate = "Test user prompt",
            Temperature = 0.7f,
            MaxDuelsPerRun = 5
        };

        await repository.CreateOrUpdateAsync(prompt, CancellationToken.None);

        // Act
        var result = await repository.GetPromptsAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Empty(result.First().Categories);
    }
}
