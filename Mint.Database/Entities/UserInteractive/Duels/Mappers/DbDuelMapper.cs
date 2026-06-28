using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Mappers;

/// <inheritdoc/>
public class DbDuelMapper : IDbEntityMapper<DuelEntity, DuelDto>
{
    /// <summary>
    /// Mapper for duel options
    /// </summary>
    private readonly IDbEntityMapper<DuelOptionEntity, DuelOptionDto> _optionMapper;

    /// <summary>
    /// Initial constructor
    /// </summary>
    /// <param name="optionMapper">Mapper for duel options</param>
    public DbDuelMapper(IDbEntityMapper<DuelOptionEntity, DuelOptionDto> optionMapper)
    {
        _optionMapper = optionMapper ?? throw new ArgumentNullException(nameof(optionMapper));
    }

    /// <inheritdoc/>
    public DuelDto Map(DuelEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new DuelDto
        {
            Id = entity.Id,
            CategoryId = entity.CategoryId,
            DuelType = entity.DuelType,
            Question = entity.Question,
            Description = entity.Description,
            ExpiresAt = entity.ExpiresAt,
            IsClosed = entity.IsClosed,
            Options = entity.Options.Select(_optionMapper.Map).ToList()
        };
    }
}
