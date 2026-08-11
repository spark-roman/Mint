using Microsoft.Extensions.DependencyInjection;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.News;
using Mint.Database.Entities.News.Dto;
using Mint.Database.Entities.News.Mappers;
using Mint.Database.Entities.News.RSS;
using Mint.Database.Entities.News.RSS.Dto;
using Mint.Database.Entities.News.RSS.Mappers;

namespace Mint.Database.Infrastructure.DI.News;

/// <summary>
/// Extension methods for news entity
/// </summary>
public static class NewsMappersExtensions
{
    /// <summary>
    /// Register news mappers
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterNewsMappers(this IServiceCollection services)
    {
        services.AddScoped<IDbEntityMapper<RssSourceEntity, RssSourceDto>, DbRssSourceMapper>();
        services.AddScoped<IDbEntityMapper<RssSourceCreateDto, RssSourceEntity>, DbRssSourceCreateMapper>();
        services.AddScoped<IDbEntityMapper<NewsEntity, NewsDto>, DbNewsMapper>();
        services.AddScoped<IDbEntityMapper<NewsCreateDto, NewsEntity>, DbNewsCreateMapper>();
    }
}
