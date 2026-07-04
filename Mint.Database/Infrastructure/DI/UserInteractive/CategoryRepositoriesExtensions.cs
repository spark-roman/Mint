using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for category repositories
/// </summary>
public static class CategoryRepositoriesExtensions
{
    /// <summary>
    /// Register category repositories
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterCategoryRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }
}