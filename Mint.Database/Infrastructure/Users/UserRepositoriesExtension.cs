using AdvApplication.Auth.Users;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.Users.Repositories;

namespace Mint.Database.Infrastructure.Users;

/// <summary>
/// User repositories extension.
/// </summary>
public static class UserRepositoriesExtension
{
    /// <summary>
    /// Register user repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterUserRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }
}
