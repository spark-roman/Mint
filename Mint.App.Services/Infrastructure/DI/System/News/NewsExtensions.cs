using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mint.App.Services.System.News.Handlers;
using Mint.App.Services.System.News.Google;
using Mint.App.Services.System.News.RSS.Handlers;
using Mint.App.Services.System.News.Tavily;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Sagara.FeedReader.Extensions;
using Gnews;

namespace Mint.App.Services.Infrastructure.DI.System.News;

/// <summary>
/// DI extension methods for News
/// </summary>
public static class NewsExtensions
{
    /// <summary>
    /// Register news services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterNewsServices(this IServiceCollection services)
    {
        services.AddFeedReaderServices();

        // Tavily
        services.AddHttpClient<ITavilyClient, TavilyClient>(client =>
        {
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        // GNews
        services.AddGnewsClient();
        services.AddScoped<IGoogleNewsClient, GoogleNewsClient>();

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IRssFeedReader, RssFeedReader>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRssNewsCollectionHandler, RssNewsCollectionHandler>();
        services.AddScoped<ITavilyNewsCollectionHandler, TavilyNewsCollectionHandler>();
        services.AddScoped<IGoogleNewsCollectionHandler, GoogleNewsCollectionHandler>();
    }
}
