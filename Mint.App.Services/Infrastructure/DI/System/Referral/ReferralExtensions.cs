using HashidsNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mint.App.Services.Infrastructure.DI.System.Referral;

/// <summary>
/// Extension methods for referral services
/// </summary>
public static class ReferralExtensions
{
    /// <summary>
    /// Register referral services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="salt">Salt</param>
    /// <param name="minLength">Minimum length</param>
    public static void RegisterReferralServices(this IServiceCollection services, string salt, int minLength = 8)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(salt);

        services.AddSingleton<IHashids>(sp =>
        {
            return new Hashids(salt, minLength);
        });
    }
}
