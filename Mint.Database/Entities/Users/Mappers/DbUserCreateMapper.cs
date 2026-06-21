using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Users.Dto;

namespace Mint.Database.Entities.Users.Mappers;

/// <inheritdoc/>
public sealed class DbUserCreateMapper : IDbEntityMapper<UserCreateDto, UserEntity>
{
    /// <inheritdoc/>
    public UserEntity Map(UserCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var userEntity = new UserEntity
        {
            ExternalUserId = entity.ExternalUserId,
            SystemType = entity.SystemType,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            UserName = entity.UserName,
            LastAuthDate = entity.LastAuthDate,
            CreatedAt = entity.CreatedAt,
            Status = (byte)UserStatus.Active
        };

        return userEntity;
    }
}