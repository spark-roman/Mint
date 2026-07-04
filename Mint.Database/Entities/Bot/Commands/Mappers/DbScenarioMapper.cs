using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Bot.Commands.Dto;

namespace Mint.Database.Entities.Bot.Commands.Mappers;

/// <summary>
/// Mapper from ScenarioEntity to ScenarioDto
/// </summary>
public sealed class DbScenarioMapper(IDbEntityMapper<StepEntity, StepDto> dbStepMapper) : IDbEntityMapper<ScenarioEntity, ScenarioDto>
{
    private readonly IDbEntityMapper<StepEntity, StepDto> _dbStepMapper = dbStepMapper ?? throw new ArgumentNullException(nameof(dbStepMapper));

    /// <inheritdoc/>
    public ScenarioDto Map(ScenarioEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new ScenarioDto
        {
            Id = entity.Id,
            Name = entity.Name,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            Steps = entity.Steps?.Select(_dbStepMapper.Map).ToList() ?? []
        };
    }
}
