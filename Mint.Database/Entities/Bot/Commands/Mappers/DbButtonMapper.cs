using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Bot.Commands.Dto;

namespace Mint.Database.Entities.Bot.Commands.Mappers;

/// <summary>
/// Mapper from ButtonEntity to ButtonDto
/// </summary>
public sealed class DbButtonMapper : IDbEntityMapper<ButtonEntity, ButtonDto>
{
    /// <inheritdoc/>
    public ButtonDto Map(ButtonEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new ButtonDto
        {
            Id = entity.Id,
            StepId = entity.ParentStepId,
            OrderNum = entity.OrderNum,
            Caption = entity.Caption,
            Action = entity.Action,
            NextStepId = entity.NextStepId
        };
    }
}
