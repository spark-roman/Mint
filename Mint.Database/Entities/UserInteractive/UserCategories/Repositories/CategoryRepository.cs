using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.Database.Entities.UserInteractive.UserCategories.Repositories;

/// <summary>
/// Repository for user categories
/// </summary>
/// <param name="categoryMapper">Mapper for category entity</param>
/// <param name="dbContextFactory">Database context factory</param>
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
}