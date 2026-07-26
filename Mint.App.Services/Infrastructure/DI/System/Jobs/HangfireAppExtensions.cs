using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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

        jobManager.AddOrUpdate(
            "duel-payout-job",
            () => serviceProvider.GetRequiredService<DuelPayoutJob>().ExecuteAsync(),
            "0 4 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }
}
