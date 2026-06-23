using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Mappers;

/// <inheritdoc/>
public class DbDuelMapper : IDbEntityMapper<DuelEntity, DuelDto>
{
    /// <inheritdoc/>
    public DuelDto Map(DuelEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DuelDto
        {
            Id = entity.Id,
            Category = entity.Category,
            Question = entity.Question,
            Description = entity.Description,
            ExpiresAt = entity.ExpiresAt,
            IsClosed = entity.IsClosed
        };
    }
}
