using Microsoft.Extensions.Logging;
using Mint.App.Services.Infrastructure.Jobs;
using Mint.App.Services.System.News.Handlers;

namespace Mint.App.Services.System.News.Jobs;

/// <summary>
/// Hangfire job for collecting news from RSS sources and Tavily API.
/// </summary>
public sealed class NewsCollectionJob(
    INewsCollector newsCollector,
    TimeProvider timeProvider,
    ILogger<NewsCollectionJob> logger) : HangfireJobBase(logger), INewsCollectionJob
{
    private readonly INewsCollector _newsCollector = newsCollector ?? throw new ArgumentNullException(nameof(newsCollector));
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc />
    public Task ExecuteAsync() =>
        ExecuteAsync(
            TimeSpan.FromMinutes(10),
            async ct =>
            {
                var targetDate = _timeProvider.GetLocalNow().Date;

                // Collect from RSS sources
                //var rssResult = await _newsCollector.CollectFromRssAsync(ct);
                //logger.LogInformation("RSS collection: {New} new, {Duplicates} duplicates", rssResult.NewSaved, rssResult.SkippedDuplicates);

                // Collect from Tavily API
                var tavilyResult = await _newsCollector.CollectFromTavilyAsync(targetDate, ct);
                logger.LogInformation("Tavily collection: {New} new, {Duplicates} duplicates", tavilyResult.NewSaved, tavilyResult.SkippedDuplicates);

                return tavilyResult.NewSaved;
            },
            "NewsCollectionJob");
}
