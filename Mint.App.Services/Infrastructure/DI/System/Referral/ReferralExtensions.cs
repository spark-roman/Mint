using HashidsNet;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
    public static void RegisterReferralServices(this IServiceCollection services)
    {
        services.AddSingleton<IHashids>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ReferralSettings>>().Value;
            return new Hashids(settings.Salt, settings.MinLength);
        });
    }
}
