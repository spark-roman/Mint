using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for vote repositories
/// </summary>
public static class VoteRepositoriesExtensions
{
    /// <summary>
    /// Register vote repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterVoteRepositories(this IServiceCollection services)
    {
        services.AddScoped<IVoteRepository, VoteRepository>();
    }
}
