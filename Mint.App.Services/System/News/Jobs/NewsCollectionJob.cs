using Microsoft.Extensions.Logging;
using Mint.App.Services.Infrastructure.Jobs;
using Mint.App.Services.System.News.Handlers;

namespace Mint.App.Services.System.News.Jobs;

/// <summary>
/// Hangfire job for collecting news from RSS sources and Tavily API.
/// </summary>
public sealed class NewsCollectionJob(
    ITavilyNewsCollectionHandler tavilyHandler,
    ILogger<NewsCollectionJob> logger) : HangfireJobBase(logger), INewsCollectionJob
{
    private readonly ITavilyNewsCollectionHandler _tavilyHandler = tavilyHandler ?? throw new ArgumentNullException(nameof(tavilyHandler));

    /// <inheritdoc />
    public Task ExecuteAsync() =>
        ExecuteAsync(
            TimeSpan.FromMinutes(10),
            async ct =>
            {
                // Collect from Tavily API
                var tavilyResult = await _tavilyHandler.CollectAsync(ct);
                logger.LogInformation("Tavily collection: {New} new, {Duplicates} duplicates", tavilyResult.NewSaved, tavilyResult.SkippedDuplicates);

                return tavilyResult.NewSaved;
            },
            "NewsCollectionJob");
}
