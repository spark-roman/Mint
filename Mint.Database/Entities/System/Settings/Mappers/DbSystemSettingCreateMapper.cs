using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Settings.Dto;

namespace Mint.Database.Entities.System.Settings.Mappers;

/// <summary>
/// Mapper from SystemSettingUpsertDto to SystemSettingEntity.
/// </summary>
public sealed class DbSystemSettingCreateMapper : IDbEntityMapper<SystemSettingUpsertDto, SystemSettingEntity>
{
    /// <inheritdoc/>
    public SystemSettingEntity Map(SystemSettingUpsertDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new SystemSettingEntity
        {
            Key = entity.Key,
            Value = entity.Value,
            Description = entity.Description,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
