using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.UserInteractive.Profiles;

namespace Mint.App.Services.Infrastructure.DI.UserInterective.Profiles;

/// <summary>
/// Register user profile services.
/// </summary>
public static class UserProfileExtensions
{
    /// <summary>
    /// Registers all user profile handlers.
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterUserProfileHandlers(this IServiceCollection services)
    {
        services.AddScoped<IUserProfilesHandler, UserProfilesHandler>();
    }
}
