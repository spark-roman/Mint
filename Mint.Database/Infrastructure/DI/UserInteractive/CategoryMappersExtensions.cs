using Mint.Common.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Mappers;

namespace Mint.Database.Infrastructure.DI.UserInteractive;

/// <summary>
/// Extension methods for category mappers
/// </summary>
public static class CategoryMappersExtensions
{
    /// <summary>
    /// Register category mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterCategoryMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<CategoryEntity, CategoryDto>, DbCategoryMapper>();
    }
}