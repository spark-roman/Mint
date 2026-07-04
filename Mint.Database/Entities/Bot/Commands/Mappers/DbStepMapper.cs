using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Bot.Commands.Dto;

namespace Mint.Database.Entities.Bot.Commands.Mappers;

/// <summary>
/// Mapper from StepEntity to StepDto
/// </summary>
public sealed class DbStepMapper(IDbEntityMapper<ButtonEntity, ButtonDto> buttonMapper) : IDbEntityMapper<StepEntity, StepDto>
{
    private readonly IDbEntityMapper<ButtonEntity, ButtonDto> _buttonMapper = buttonMapper ?? throw new ArgumentNullException(nameof(buttonMapper));

    /// <inheritdoc/>
    public StepDto Map(StepEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new StepDto
        {
            Id = entity.Id,
            ScenarioId = entity.ScenarioId,
            OrderNum = entity.OrderNum,
            StepTypeId = entity.StepTypeId,
            Message = entity.Message,
            IsFinal = entity.IsFinal,
            Data = entity.Data,
            Buttons = entity.Buttons?.Select(_buttonMapper.Map).ToList() ?? []
        };
    }
}
