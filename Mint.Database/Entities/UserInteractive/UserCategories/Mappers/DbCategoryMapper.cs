using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.Database.Entities.UserInteractive.UserCategories.Mappers;

/// <summary>
/// Mapper for category entity
/// </summary>
public class DbCategoryMapper : IDbEntityMapper<CategoryEntity, CategoryDto>
{
    /// <inheritdoc/>
    public CategoryDto Map(CategoryEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsActiveForAI = entity.IsActiveForAI,
            SearchKeywords = entity.SearchKeywords
        };
    }
}
