using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Users;
using Mint.Database.Entities.Users.Dto;
using Mint.Database.Entities.Users.Mappers;
using Mint.Database.Entities.Users.Sessions;
using Mint.Database.Entities.Users.Sessions.Dto;
using Mint.Database.Entities.Users.Sessions.Mappers;

namespace Mint.Database.Infrastructure.DI.Users;

/// <summary>
/// Extension methods for user entity
/// </summary>
public static class UserMappersExtensions
{
    /// <summary>
    /// Register user mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterUserMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<UserCreateDto, UserEntity>, DbUserCreateMapper>();
        services.AddSingleton<IDbEntityMapper<UserEntity, UserDto>,DbUserMapper>();

        services.AddSingleton<IDbEntityMapper<UserSessionEntity, UserSessionDto>, DbUserSessionMapper>();
        services.AddSingleton<IDbEntityMapper<UserSessionDto, UserSessionEntity>, DbUserSessionCreateMapper>();
    }
}
