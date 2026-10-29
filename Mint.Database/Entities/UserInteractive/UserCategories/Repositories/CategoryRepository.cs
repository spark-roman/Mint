using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.Database.Entities.UserInteractive.UserCategories.Repositories;

/// <summary>
/// Repository for user categories
/// </summary>
public sealed class CategoryRepository(
    IDbEntityMapper<CategoryEntity, CategoryDto> categoryMapper,
    IDbContextFactory<MintDbContext> dbContextFactory) : ICategoryRepository
{
    private readonly IDbEntityMapper<CategoryEntity, CategoryDto> _categoryMapper = categoryMapper ?? throw new ArgumentNullException(nameof(categoryMapper));

    private readonly IDbContextFactory<MintDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

    /// <inheritdoc/>
    public async Task<List<CategoryDto>> GetAllActiveAsync(CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var categories = await context.UserCategories
            .Where(c => c.IsActiveForAI)
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(_categoryMapper.Map).ToList();
    }

    /// <inheritdoc/>
    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var category = await context.UserCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return category is null ? null : _categoryMapper.Map(category);
    }

    /// <inheritdoc/>
    public async Task<CategoryDto?> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(name);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var category = await context.UserCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Name == name, cancellationToken);

        return category is null ? null : _categoryMapper.Map(category);
    }

    /// <inheritdoc/>
    public async Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var entity = new CategoryEntity
        {
            Name = dto.Name,
            Description = dto.Description,
            IsActiveForAI = dto.IsActiveForAI,
            SearchKeywords = dto.SearchKeywords,
            Code = dto.Code
        };

        await context.UserCategories.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return _categoryMapper.Map(entity);
    }

    /// <inheritdoc/>
    public async Task<CategoryDto?> UpdateAsync(int id, CategoryDto dto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(dto);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var category = await context.UserCategories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null)
        {
            return null;
        }

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.IsActiveForAI = dto.IsActiveForAI;
        category.SearchKeywords = dto.SearchKeywords;
        category.Code = dto.Code;

        await context.SaveChangesAsync(cancellationToken);

        return _categoryMapper.Map(category);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var category = await context.UserCategories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category is null)
        {
            return false;
        }

        context.UserCategories.Remove(category);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc/>
    public async Task<CategoryDto?> GetByCodeAsync(string code, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(code);

        using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var category = await context.UserCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

        return category is null ? null : _categoryMapper.Map(category);
    }

    /// <inheritdoc/>
    public async Task<List<CategoryStatusDto>> GetCategoriesWithDuelStatusAsync(long externalUserId, DateTimeOffset expireDate, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var categories = await context.UserCategories
            .AsNoTracking()
            .Where(c => c.IsActiveForAI)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        var categoryIds = categories.Select(c => c.Id).ToList();

        var duels = await context.Duels
            .AsNoTracking()
            .Where(d => categoryIds.Contains(d.CategoryId))
            .Include(d => d.Votes)
            .ToListAsync(cancellationToken);

        var userAccountId = await context.Accounts
            .AsNoTracking()
            .Where(a => a.User.ExternalUserId == externalUserId)
            .Select(a => a.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return categories.Select(c =>
        {
            var categoryDuels = duels.Where(d => d.CategoryId == c.Id).ToList();

            var hasAvailableDuels = categoryDuels.Any(d =>
                d.Status == DuelStatus.Active
                && d.ExpiresAt > expireDate
                && !d.Votes.Any(v => v.AccountId == userAccountId));

            var hasExpiredDuels = categoryDuels.Any(d =>
                d.Status == DuelStatus.Active
                && d.ExpiresAt <= expireDate);

            var hasClosedDuels = categoryDuels.Any(d =>
                d.Status == DuelStatus.Closed);

            var statusEmoji = GetCategoryStatusEmoji(hasAvailableDuels, hasExpiredDuels, hasClosedDuels);

            return new CategoryStatusDto
            {
                CategoryId = c.Id,
                CategoryName = c.Name,
                CategoryCode = c.Code,
                CategoryEmoji = "📂",
                IsActive = c.IsActiveForAI,
                HasAvailableDuels = hasAvailableDuels,
                HasExpiredDuels = hasExpiredDuels,
                HasClosedDuels = hasClosedDuels,
                StatusEmoji = statusEmoji
            };
        }).ToList();
    }

    private static string GetCategoryStatusEmoji(bool hasAvailableDuels, bool hasExpiredDuels, bool hasClosedDuels)
    {
        return hasAvailableDuels ? "🔥" : hasExpiredDuels ? "⏳" : hasClosedDuels ? "🏁" : "📂";
    }
}
