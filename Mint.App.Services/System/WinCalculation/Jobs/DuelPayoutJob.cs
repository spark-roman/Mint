using Microsoft.Extensions.Logging;
using Mint.App.Services.Infrastructure.Jobs;
using Mint.App.Services.System.WinCalculation.Services;

namespace Mint.App.Services.System.WinCalculation.Jobs;

/// <summary>
/// Hangfire job for processing duel payouts.
/// </summary>
public sealed class DuelPayoutJob(IDuelPayoutService payoutService, ILogger<DuelPayoutJob> logger)
    : HangfireJobBase(logger), IDuelPayoutJob
{
    private readonly IDuelPayoutService _payoutService = payoutService
        ?? throw new ArgumentNullException(nameof(payoutService));

    /// <inheritdoc />
    public Task ExecuteAsync() =>
        ExecuteAsync(
            TimeSpan.FromMinutes(30),
            ct => _payoutService.ProcessExpiredDuelsPayoutsAsync(ct),
            "DuelPayoutJob");
}
