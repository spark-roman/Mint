using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.DuelsGeneration.Jobs;
using Mint.App.Services.System.News.Jobs;
using Mint.App.Services.System.WinCalculation.Jobs;

namespace Mint.App.Services.Infrastructure.DI.System.Jobs;

/// <summary>
/// Hangfire service extensions
/// </summary>
public static class HangfireAppExtensions
{
    /// <summary>
    /// Schedule recurring jobs
    /// </summary>
    /// <param name="serviceProvider"></param>
    public static void ScheduleRecurringJobs(this IServiceProvider serviceProvider)
    {
        var jobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();

        // News collection: 06:00 UTC daily
        jobManager.AddOrUpdate(
            "news-collection-job",
            () => serviceProvider.GetRequiredService<NewsCollectionJob>().ExecuteAsync(),
            "0 3 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        // Duel generation: 23:30 MSK = 20:30 UTC daily
        /*jobManager.AddOrUpdate(
            "duel-generation-job",
            () => serviceProvider.GetRequiredService<DuelGenerationJob>().ExecuteAsync(),
            "30 20 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });*/

        // Duel payout pipeline: 04:00 UTC daily
        jobManager.AddOrUpdate(
            "duel-jobs",
            () => serviceProvider.GetRequiredService<DuelPayoutPipelineJob>().ExecuteAsync(),
            "0 4 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });

        jobManager.RemoveIfExists("duel-payout-job");
        jobManager.RemoveIfExists("payout-execution-job");
        jobManager.RemoveIfExists("duel-publish-job");
    }
}
