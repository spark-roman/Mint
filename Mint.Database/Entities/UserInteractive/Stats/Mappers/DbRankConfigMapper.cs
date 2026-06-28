using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Mappers;

/// <summary>
/// Mapper for rank config entity
/// </summary>
public class DbRankConfigMapper : IDbEntityMapper<RankConfigEntity, RankConfigDto>
{
    /// <inheritdoc/>
    public RankConfigDto Map(RankConfigEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new RankConfigDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            Emoji = entity.Emoji,
            MinPoints = entity.MinPoints
        };
    }
}
