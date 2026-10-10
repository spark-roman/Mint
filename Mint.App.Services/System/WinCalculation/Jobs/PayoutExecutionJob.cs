using Microsoft.Extensions.Logging;
using Mint.App.Services.System.WinCalculation.Services;

namespace Mint.App.Services.System.WinCalculation.Jobs;

/// <summary>
/// Hangfire job for executing payout plans that the settlement phase left as pending.
/// </summary>
public sealed class PayoutExecutionJob(IDuelPayoutService payoutService, ILogger<PayoutExecutionJob> logger)
{
    private readonly IDuelPayoutService _payoutExecutionService = payoutService
        ?? throw new ArgumentNullException(nameof(payoutService));

    private readonly ILogger<PayoutExecutionJob> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Executes the payout execution job.
    /// </summary>
    public async Task ExecuteAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(15));

        try
        {
            _logger.LogInformation("PayoutExecutionJob started at {Time}", DateTimeOffset.UtcNow);

            var count = await _payoutExecutionService.ExecutePendingPayoutsAsync(cts.Token);

            _logger.LogInformation("PayoutExecutionJob completed. Executed {Count} payouts.", count);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("PayoutExecutionJob was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PayoutExecutionJob failed");
            throw;
        }
    }
}
