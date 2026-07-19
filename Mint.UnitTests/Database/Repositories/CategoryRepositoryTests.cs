using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.UnitTests.Database.Fixtures.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.UserInteractive;

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

        #region GetAllActiveAsync

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
        await _fixture.ResetAsync();
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

    #endregion

    #region GetCategoriesWithDuelStatusAsync

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns all seeded categories.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_ReturnsAllCategories()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var futureDate = DateTimeOffset.UtcNow.AddHours(24);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, futureDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Count >= 12);
    }

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns categories with correct names.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_ReturnsCorrectCategoryNames()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var futureDate = DateTimeOffset.UtcNow.AddHours(24);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, futureDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var names = result.Select(c => c.CategoryName).ToList();

        Assert.Contains("Крипта & Web3", names);
        Assert.Contains("Кино & Сериалы", names);
    }

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns correct category codes.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_ReturnsCorrectCategoryCodes()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var futureDate = DateTimeOffset.UtcNow.AddHours(24);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, futureDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var codes = result.Select(c => c.CategoryCode).ToList();
        
        Assert.Contains("crypto", codes);
        Assert.Contains("movies", codes);
    }

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns IsActive = true for all categories.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_AllCategoriesActive()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var futureDate = DateTimeOffset.UtcNow.AddHours(24);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, futureDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        foreach (var category in result)
        {
            Assert.True(category.IsActive);
        }
    }

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns HasAvailableDuels = false
    /// when there are no duels in the database.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_NoDuels_ReturnsFalseForAllCategories()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var futureDate = DateTimeOffset.UtcNow.AddHours(24);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, futureDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        foreach (var category in result.Where(c => c.CategoryId > 1))
        {
            Assert.False(category.HasAvailableDuels);
            Assert.Equal("🔴", category.StatusEmoji);
        }
    }

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns HasAvailableDuels = true
    /// for categories that have active (not expired, not closed) duels.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_WithActiveDuels_ReturnsTrueForMatchingCategories()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<Mint.Database.MintDbContext>();
        var now = DateTimeOffset.UtcNow;
        var futureDate = now.AddHours(24);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, now, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var aiCategory = result.First(c => c.CategoryCode == "ai");
        Assert.True(aiCategory.HasAvailableDuels);
        Assert.Equal("🟢", aiCategory.StatusEmoji);
    }

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns HasAvailableDuels = false
    /// for categories where all duels are expired.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_WithExpiredDuels_ReturnsFalseForMatchingCategories()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<Mint.Database.MintDbContext>();
        var futureDate = DateTimeOffset.UtcNow.AddHours(24);
        var pastDate = DateTimeOffset.UtcNow.AddHours(-1);

        // Create an expired duel in category 1
        var duel = new DuelEntity
        {
            Id = 1,
            CategoryId = 12,
            DuelType = DuelType.OpinionMatch,
            Question = "Вопрос из прошлого",
            Description = "Устаревший вопрос",
            ExpiresAt = pastDate,
            IsClosed = false,
            Options =
            [
                new DuelOptionEntity { Id = 1, OptionText = "Да", OptionCode = "yes" },
                new DuelOptionEntity { Id = 2, OptionText = "Нет", OptionCode = "no" }
            ]
        };
        dbContext.Duels.Add(duel);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, futureDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var aiCategory = result.First(c => c.CategoryCode == "finance");
        Assert.False(aiCategory.HasAvailableDuels);
        Assert.Equal("🔴", aiCategory.StatusEmoji);
    }

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns HasAvailableDuels = false
    /// for categories where all duels are closed.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_WithClosedDuels_ReturnsFalseForMatchingCategories()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<Mint.Database.MintDbContext>();
        var futureDate = DateTimeOffset.UtcNow.AddHours(24);

        // Create a closed duel in category 2
        var duel = new DuelEntity
        {
            Id = 1,
            CategoryId = 2,
            DuelType = DuelType.OpinionMatch,
            Question = "Закрытый спор",
            Description = "Завершённый спор",
            ExpiresAt = futureDate,
            IsClosed = true,
            Options =
            [
                new DuelOptionEntity { Id = 1, OptionText = "Да", OptionCode = "yes" },
                new DuelOptionEntity { Id = 2, OptionText = "Нет", OptionCode = "no" }
            ]
        };
        dbContext.Duels.Add(duel);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, futureDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var hardwareCategory = result.First(c => c.CategoryCode == "hardware");
        Assert.False(hardwareCategory.HasAvailableDuels);
        Assert.Equal("🔴", hardwareCategory.StatusEmoji);
    }

    /// <summary>
    /// Verifies that GetCategoriesWithDuelStatusAsync returns correct category IDs.
    /// </summary>
    [Fact]
    public async Task GetCategoriesWithDuelStatusAsync_ReturnsCorrectCategoryIds()
    {
        // Arrange
        await _fixture.ResetAsync();
        using var scope = _fixture.ServiceProvider.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<ICategoryRepository>();
        var futureDate = DateTimeOffset.UtcNow.AddHours(24);

        // Act
        var result = await repository.GetCategoriesWithDuelStatusAsync(1001, futureDate, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var ids = result.Select(c => c.CategoryId).ToList();
        Assert.Contains(1, ids);
        Assert.Contains(2, ids);
        Assert.Contains(12, ids);
    }

    #endregion
}