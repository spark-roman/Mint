using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.News.Handlers;
using Mint.App.Services.System.News.Google;
using Mint.App.Services.System.News.RSS.Handlers;
using Mint.App.Services.System.News.Tavily;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Sagara.FeedReader.Extensions;

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
    /// <param name="configuration">Application configuration</param>
    public static void RegisterNewsServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        services.AddFeedReaderServices();

        // Tavily
        services.AddHttpClient<ITavilyClient, TavilyClient>(client =>
        {
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        // GNews — custom HTTP client, no external package
        services.AddHttpClient<IGoogleNewsClient, GoogleNewsClient>(client =>
        {
            client.BaseAddress = new Uri("https://gnews.io/");
            client.Timeout = TimeSpan.FromMinutes(2);
        });

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IRssFeedReader, RssFeedReader>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IRssNewsCollectionHandler, RssNewsCollectionHandler>();
        services.AddScoped<ITavilyNewsCollectionHandler, TavilyNewsCollectionHandler>();
        services.AddScoped<IGoogleNewsCollectionHandler, GoogleNewsCollectionHandler>();
    }
}
