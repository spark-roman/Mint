using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Mappers;

/// <summary>
/// Mapper for duel option creation
/// </summary>
public class DbDuelOptionCreateMapper : IDbEntityMapper<DuelOptionCreateDto, DuelOptionEntity>
{
    /// <inheritdoc/>
    public DuelOptionEntity Map(DuelOptionCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DuelOptionEntity
        {
            OptionText = entity.OptionText,
            OptionCode = entity.OptionCode
        };
    }
}
