using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.News.Handlers;
using Mint.App.Services.System.News.RSS.Handlers;
using Sagara.FeedReader;
using Sagara.FeedReader.Extensions;
using Sagara.FeedReader.Http;

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

        services.AddScoped<IRssFeedReader, RssFeedReader>();
        services.AddScoped<INewsCollector, NewsCollector>();
    }
}
