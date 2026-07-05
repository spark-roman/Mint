using Mint.Database.Entities.UserInteractive.UserCategories.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.UnitTests.Database.Repositories;

/// <summary>
/// Tests for <see cref="CategoryRepository"/>
/// </summary>
public class CategoryRepositoryTests : IClassFixture<RepositoryFixture>
{
    private readonly RepositoryFixture _fixture;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="fixture">Repository fixture</param>
    public CategoryRepositoryTests(RepositoryFixture fixture)
    {
        ArgumentNullException.ThrowIfNull(fixture);
        _fixture = fixture;
    }

    /// <summary>
    /// Verifies that getting all active categories returns the seeded categories.
    /// </summary>
    [Fact]
    public async Task GetAllActiveAsync_ReturnsSeededCategories()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act
        var result = await repository.GetAllActiveAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count >= 12);
    }

    /// <summary>
    /// Verifies that getting a category by ID returns the correct category.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCategory()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act
        var result = await repository.GetByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Нейросети & ИИ", result.Name);
        Assert.True(result.IsActiveForAI);
    }

    /// <summary>
    /// Verifies that getting a category by non-existent ID returns null.
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_NonExistentId_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act
        var result = await repository.GetByIdAsync(999, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that getting a category by name returns the correct category.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_ExistingName_ReturnsCategory()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act
        var result = await repository.GetByNameAsync("Крипта & Web3", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Крипта & Web3", result.Name);
        Assert.True(result.IsActiveForAI);
    }

    /// <summary>
    /// Verifies that getting a category by non-existent name returns null.
    /// </summary>
    [Fact]
    public async Task GetByNameAsync_NonExistentName_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act
        var result = await repository.GetByNameAsync("NonExistentCategory", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that creating a new category returns a valid category with generated ID.
    /// </summary>
    [Fact]
    public async Task CreateAsync_NewCategory_CreatesSuccessfully()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        var category = new CategoryDto
        {
            Name = "Тестовая категория",
            Description = "Описание тестовой категории",
            IsActiveForAI = true,
            SearchKeywords = "тест, проверка",
            Code = "TEST"
        };

        // Act
        var result = await repository.CreateAsync(category, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Тестовая категория", result.Name);
        Assert.Equal("Описание тестовой категории", result.Description);
        Assert.True(result.IsActiveForAI);
        Assert.Equal("тест, проверка", result.SearchKeywords);
    }

    /// <summary>
    /// Verifies that creating a category with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task CreateAsync_NullCategory_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.CreateAsync(null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that updating an existing category modifies the data correctly.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ExistingCategory_UpdatesSuccessfully()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Create a new category first (seed data may be modified by other tests)
        var newCategory = new CategoryDto
        {
            Name = "Тестовая категория для обновления",
            Description = "Описание",
            IsActiveForAI = true,
            SearchKeywords = "ключи",
            Code = "UPD_TEST"
        };

        var created = await repository.CreateAsync(newCategory, CancellationToken.None);
        Assert.NotNull(created);

        var updatedCategory = new CategoryDto
        {
            Id = created.Id,
            Name = "Обновлённая категория",
            Description = "Новое описание",
            IsActiveForAI = false,
            SearchKeywords = "обновлённые ключи",
            Code = "UPD"
        };

        // Act
        var result = await repository.UpdateAsync(created.Id, updatedCategory, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
        Assert.Equal("Обновлённая категория", result.Name);
        Assert.Equal("Новое описание", result.Description);
        Assert.False(result.IsActiveForAI);
        Assert.Equal("обновлённые ключи", result.SearchKeywords);
    }

    /// <summary>
    /// Verifies that updating a non-existent category returns null.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_NonExistentCategory_ReturnsNull()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        var updatedCategory = new CategoryDto
        {
            Id = 999,
            Name = "Не существует",
            Description = "Описание",
            IsActiveForAI = true,
            SearchKeywords = "ключи",
            Code = "NEI"
        };

        // Act
        var result = await repository.UpdateAsync(999, updatedCategory, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that updating a category with null DTO throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_NullCategory_ThrowsArgumentNullException()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await repository.UpdateAsync(1, null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that deleting an existing category returns true.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_ExistingCategory_DeletesSuccessfully()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act
        var result = await repository.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that deleting a non-existent category returns false.
    /// </summary>
    [Fact]
    public async Task DeleteAsync_NonExistentCategory_ReturnsFalse()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act
        var result = await repository.DeleteAsync(999, CancellationToken.None);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that getting all active categories returns them ordered by name.
    /// </summary>
    [Fact]
    public async Task GetAllActiveAsync_ReturnsOrderedByCategoryName()
    {
        // Arrange
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Act
        var result = await repository.GetAllActiveAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        for (int i = 1; i < result.Count; i++)
        {
            Assert.True(string.Compare(result[i - 1].Name, result[i].Name, StringComparison.Ordinal) < 0);
        }
    }
}