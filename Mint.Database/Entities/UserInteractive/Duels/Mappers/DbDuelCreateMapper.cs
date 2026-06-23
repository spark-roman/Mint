using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Mappers;

/// <inheritdoc/>
public class DbDuelCreateMapper : IDbEntityMapper<DuelCreateDto, DuelEntity>
{
    /// <inheritdoc/>
    public DuelEntity Map(DuelCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DuelEntity
        {
            Category = entity.Category,
            Question = entity.Question,
            Description = entity.Description,
            ExpiresAt = entity.ExpiresAt,
            IsClosed = false
        };
    }
}
