using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Users.Dto;

namespace Mint.Database.Entities.Users.Mappers;

/// <inheritdoc/>
public class DbUserMapper : IDbEntityMapper<UserEntity, UserDto>
{
    /// <inheritdoc/>
    public UserDto Map(UserEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var userDto = new UserDto
        {
            Id = entity.Id,
            ExternalUserId = entity.ExternalUserId,
            SystemType = entity.SystemType,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            UserName = entity.UserName,
            LastAuthDate = entity.LastAuthDate,
            CreatedAt = entity.CreatedAt
        };

        return userDto;
    }
}
