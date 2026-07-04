using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Commands.Mappers;
using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Users.Dto;
using Telegram.Bot.Types;

namespace Mint.App.Services.Infrastructure.DI.System;

/// <summary>
/// DI extension methods for start command
/// </summary>
public static class StartCommandsExtensions
{
    /// <summary>
    /// Register start command mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void AddStartCommandMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDtoMapper<User, UserCreateDto>, TgUserCreateMapper>();
        services.AddScoped<IDtoMapper<User, ExternalUserDto>, TgUserMapper>();
    }
}
