using Microsoft.Extensions.Logging;
using Mint.App.Services.Infrastructure.Jobs;
using Mint.App.Services.System.DuelsGeneration.Jobs;

namespace Mint.App.Services.System.WinCalculation.Jobs;

/// <summary>
/// Combined Hangfire job for running duel payout pipeline sequentially.
/// </summary>
public sealed class DuelPayoutPipelineJob(
    IDuelPayoutJob duelPayoutJob,
    IPayoutExecutionJob payoutExecutionJob,
    IDuelPublishJob duelPublishJob,
    ILogger<DuelPayoutPipelineJob> logger)
    : HangfireJobBase(logger)
{
    private readonly IDuelPayoutJob _duelPayoutJob = duelPayoutJob
        ?? throw new ArgumentNullException(nameof(duelPayoutJob));

    private readonly IPayoutExecutionJob _payoutExecutionJob = payoutExecutionJob
        ?? throw new ArgumentNullException(nameof(payoutExecutionJob));

    private readonly IDuelPublishJob _duelPublishJob = duelPublishJob
        ?? throw new ArgumentNullException(nameof(duelPublishJob));

    /// <summary>
    /// Executes the full pipeline: payout → execution → publish.
    /// </summary>
    public async Task ExecuteAsync() =>
        await ExecuteAsync(
            TimeSpan.FromHours(2),
            async ct =>
            {
                await _duelPayoutJob.ExecuteAsync();
                await _payoutExecutionJob.ExecuteAsync();
                await _duelPublishJob.ExecuteAsync();

                return 1;
            },
            "DuelPayoutPipelineJob");
}
