using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Mappers;
using Mint.Database.Entities.Users.Sessions.Dto;

namespace Mint.Database.Entities.Users.Sessions.Mappers;

/// <summary>
/// Mapper from UserSessionEntity to UserSessionDto
/// </summary>
public sealed class DbUserSessionMapper(IDbEntityMapper<StepEntity, StepDto> stepMapper) : IDbEntityMapper<UserSessionEntity, UserSessionDto>
{
    private readonly IDbEntityMapper<StepEntity, StepDto> _stepMapper = stepMapper ?? throw new ArgumentNullException(nameof(stepMapper));

    /// <inheritdoc/>
    public UserSessionDto Map(UserSessionEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new UserSessionDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ScenarioId = entity.ScenarioId,
            CurrentStepId = entity.CurrentStepId,
            Data = entity.Data,
            StartedAt = entity.StartedAt,
            CompletedAt = entity.CompletedAt,
            CurrentStep = entity.CurrentStep != null ? _stepMapper.Map(entity.CurrentStep) : null
        };
    }
}
