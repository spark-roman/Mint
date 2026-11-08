using Microsoft.Extensions.Logging;
using Mint.App.Services.Infrastructure.Jobs;
using Mint.App.Services.System.News.Handlers;

namespace Mint.App.Services.System.News.Jobs;

/// <summary>
/// Hangfire job for collecting news from RSS sources, Tavily API, and GNews API.
/// </summary>
public sealed class NewsCollectionJob(
    IGoogleNewsCollectionHandler googleHandler,
    ILogger<NewsCollectionJob> logger) : HangfireJobBase(logger), INewsCollectionJob
{
    private readonly IGoogleNewsCollectionHandler _googleHandler = googleHandler ?? throw new ArgumentNullException(nameof(googleHandler));

    /// <inheritdoc />
    public Task ExecuteAsync() =>
        ExecuteAsync(
            TimeSpan.FromMinutes(10),
            async ct =>
            {
                // Collect from GNews API
                var googleResult = await _googleHandler.CollectAsync(ct);
                logger.LogInformation("GNews collection: {New} new, {Duplicates} duplicates", googleResult.NewSaved, googleResult.SkippedDuplicates);

                return googleResult.NewSaved;
            },
            "NewsCollectionJob");
}
