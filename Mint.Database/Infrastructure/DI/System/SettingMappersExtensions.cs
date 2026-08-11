using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Settings;
using Mint.Database.Entities.System.Settings.Dto;
using Mint.Database.Entities.System.Settings.Mappers;

namespace Mint.Database.Infrastructure.DI.System;

/// <summary>
/// Setting mappers registration
/// </summary>
public static class SettingMappersExtensions
{
    /// <summary>
    /// Register AI prompt mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterSettingMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<SystemSettingUpsertDto, SystemSettingEntity>, DbSystemSettingCreateMapper>();
        services.AddSingleton<IDbEntityMapper<SystemSettingEntity, SystemSettingDto>, DbSystemSettingMapper>();
    }
}
