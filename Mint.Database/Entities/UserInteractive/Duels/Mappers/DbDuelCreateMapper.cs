using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Mappers;

/// <inheritdoc/>
public class DbDuelCreateMapper : IDbEntityMapper<DuelCreateDto, DuelEntity>
{
    /// <summary>
    /// Mapper for duel options
    /// </summary>
    private readonly IDbEntityMapper<DuelOptionCreateDto, DuelOptionEntity> _optionMapper;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="optionMapper">Mapper for duel options</param>
    public DbDuelCreateMapper(IDbEntityMapper<DuelOptionCreateDto, DuelOptionEntity> optionMapper)
    {
        _optionMapper = optionMapper ?? throw new ArgumentNullException(nameof(optionMapper));
    }

    /// <inheritdoc/>
    public DuelEntity Map(DuelCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var duelEntity = new DuelEntity
        {
            CategoryId = entity.CategoryId,
            DuelType = entity.DuelType,
            Question = entity.Question,
            Description = entity.Description,
            ExpiresAt = entity.ExpiresAt,
            IsClosed = false,
            Options = entity.Options.Select(_optionMapper.Map).ToList()
        };

        return duelEntity;
    }
}
