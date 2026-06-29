using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Users.Sessions.Dto;

namespace Mint.Database.Entities.Users.Sessions.Mappers;

/// <summary>
/// Mapper from UserSessionDto to UserSessionEntity (for create/update)
/// </summary>
public sealed class DbUserSessionCreateMapper : IDbEntityMapper<UserSessionDto, UserSessionEntity>
{
    /// <inheritdoc/>
    public UserSessionEntity Map(UserSessionDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new UserSessionEntity
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ScenarioId = entity.ScenarioId,
            CurrentStepId = entity.CurrentStepId,
            Data = entity.Data,
            StartedAt = entity.StartedAt == default ? DateTimeOffset.UtcNow : entity.StartedAt,
            CompletedAt = entity.CompletedAt
        };
    }
}
