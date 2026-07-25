using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.News.Repositories;
using Mint.Database.Entities.News.RSS.Repositories;

namespace Mint.Database.Infrastructure.DI.News;

/// <summary>
/// Extension methods for news repositories
/// </summary>
public static class NewsRepositoriesExtensions
{
    /// <summary>
    /// Register news repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterNesRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRssSourceRepository, RssSourceRepository>();
        services.AddScoped<INewsRepository, NewsRepository>();
    }
}
