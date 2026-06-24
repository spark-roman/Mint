using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.Database.Entities.System.Mappers;

/// <summary>
/// Mapper for AI prompt entity
/// </summary>
public class DbAiPromptMapper : IDbEntityMapper<AiPromptEntity, AiPromptDto>
{
    /// <summary>
    /// Mapper for categories
    /// </summary>
    private readonly IDbEntityMapper<CategoryEntity, CategoryDto> _categoryMapper;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="categoryMapper">Mapper for categories</param>
    public DbAiPromptMapper(IDbEntityMapper<CategoryEntity, CategoryDto> categoryMapper)
    {
        _categoryMapper = categoryMapper ?? throw new ArgumentNullException(nameof(categoryMapper));
    }

    /// <inheritdoc/>
    public AiPromptDto Map(AiPromptEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AiPromptDto
        {
            Id = entity.Id,
            SystemCoreText = entity.SystemCoreText,
            Temperature = entity.Temperature,
            MaxDuelsPerRun = entity.MaxDuelsPerRun,
            UpdatedAt = entity.UpdatedAt,
            Categories = [.. entity.Categories.Select(_categoryMapper.Map)]
        };
    }
}
