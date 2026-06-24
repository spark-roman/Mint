using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Mappers;

/// <summary>
/// Mapper for duel option
/// </summary>
public class DbDuelOptionMapper : IDbEntityMapper<DuelOptionEntity, DuelOptionDto>
{
    /// <inheritdoc/>
    public DuelOptionDto Map(DuelOptionEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DuelOptionDto
        {
            Id = entity.Id,
            OptionText = entity.OptionText,
            OptionCode = entity.OptionCode
        };
    }
}
