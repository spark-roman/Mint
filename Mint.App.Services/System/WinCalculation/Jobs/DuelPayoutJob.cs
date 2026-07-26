using Microsoft.Extensions.Logging;
using Mint.App.Services.System.WinCalculation.Services;

namespace Mint.App.Services.System.WinCalculation.Jobs;

/// <summary>
/// Hangfire job for processing duel payouts.
/// </summary>
public sealed class DuelPayoutJob(IDuelPayoutService payoutService, ILogger<DuelPayoutJob> logger)
{
    private readonly IDuelPayoutService _payoutService = payoutService ?? throw new ArgumentNullException(nameof(payoutService));
    private readonly ILogger<DuelPayoutJob> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Executes the duel payout job.
    /// </summary>
    public async Task ExecuteAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(30));

        try
        {
            _logger.LogInformation("DuelPayoutJob started at {Time}", DateTimeOffset.UtcNow);

            var count = await _payoutService.ProcessExpiredDuelsPayoutsAsync(cts.Token);

            _logger.LogInformation("DuelPayoutJob completed. Processed {Count} duels.", count);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("DuelPayoutJob was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DuelPayoutJob failed");
            throw;
        }
    }
}
