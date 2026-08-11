using Mint.Common.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;
using Mint.Database.Entities.UserInteractive.UserCategories;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Mappers;
using Mint.Database.Entities.System.Prompts.Dto;
using Mint.Database.Entities.Prompts.System;
using Mint.Database.Entities.System.Prompts.Mappers;

namespace Mint.Database.Infrastructure.DI.System;

/// <summary>
/// Extension methods for AI prompt mappers
/// </summary>
public static class AiPromptMappersExtensions
{
    /// <summary>
    /// Register AI prompt mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterAiPromptMappers(this IServiceCollection services)
    {
        services.AddSingleton<IDbEntityMapper<AiPromptCreateDto, AiPromptEntity>, DbAiPromptCreateMapper>();
        services.AddSingleton<IDbEntityMapper<AiPromptEntity, AiPromptDto>, DbAiPromptMapper>();
        services.AddSingleton<IDbEntityMapper<CategoryEntity, CategoryDto>, DbCategoryMapper>();
    }
}
