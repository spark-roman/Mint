using Microsoft.Extensions.Logging;
using Mint.App.Services.Infrastructure.Jobs;
using Mint.App.Services.System.WinCalculation.Services;

namespace Mint.App.Services.System.WinCalculation.Jobs;

/// <summary>
/// Hangfire job for executing payout plans that the settlement phase left as pending.
/// </summary>
public sealed class PayoutExecutionJob(IDuelPayoutService payoutService, ILogger<PayoutExecutionJob> logger)
    : HangfireJobBase(logger)
{
    private readonly IDuelPayoutService _payoutService = payoutService
        ?? throw new ArgumentNullException(nameof(payoutService));

    /// <inheritdoc />
    public Task ExecuteAsync() =>
        ExecuteAsync(
            TimeSpan.FromMinutes(15),
            ct => _payoutService.ExecutePendingPayoutsAsync(ct),
            "PayoutExecutionJob");
}
