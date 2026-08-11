using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Settings.Dto;

namespace Mint.Database.Entities.System.Settings.Mappers;

/// <summary>
/// Mapper from SystemSettingEntity to SystemSettingDto.
/// </summary>
public sealed class DbSystemSettingMapper : IDbEntityMapper<SystemSettingEntity, SystemSettingDto>
{
    /// <inheritdoc/>
    public SystemSettingDto Map(SystemSettingEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new SystemSettingDto
        {
            Id = entity.Id,
            Key = entity.Key,
            Value = entity.Value,
            Description = entity.Description,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
