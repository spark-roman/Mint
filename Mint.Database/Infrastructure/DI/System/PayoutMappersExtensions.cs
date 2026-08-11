using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Payouts;
using Mint.Database.Entities.System.Payouts.Dto;
using Mint.Database.Entities.System.Payouts.Mappers;

namespace Mint.Database.Infrastructure.DI.System;

/// <summary>
/// Payout mappers registration
/// </summary>
public static class PayoutMappersExtensions
{
    /// <summary>
    /// Register payout mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterPayoutMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<PayoutCreateDto, PayoutEntity>, DbPayoutCreateMapper>();
        services.AddSingleton<IDbEntityMapper<PayoutEntity, PayoutDto>, DbPayoutMapper>();
    }
}
