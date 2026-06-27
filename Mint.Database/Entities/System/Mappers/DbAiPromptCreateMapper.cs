using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Dto;

namespace Mint.Database.Entities.System.Mappers;

/// <summary>
/// Mapper for creating AI prompt entity
/// </summary>
public class DbAiPromptCreateMapper : IDbEntityMapper<AiPromptCreateDto, AiPromptEntity>
{
    /// <inheritdoc/>
    public AiPromptEntity Map(AiPromptCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AiPromptEntity
        {
            SystemPromptTemplate = entity.SystemPromptTemplate,
            UserPromptTemplate = entity.UserPromptTemplate,
            Temperature = entity.Temperature,
            MaxDuelsPerRun = entity.MaxDuelsPerRun,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
