using Microsoft.Extensions.Logging;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <inheritdoc cref="IDuelSettlementHandler"/>
public sealed class DuelSettlementHandler(
    IPayoutPlanningHandler payoutPlanningHandler,
    IPayoutExecutionHandler payoutExecutionHandler,
    ILogger<DuelSettlementHandler> logger) : IDuelSettlementHandler
{
    private readonly IPayoutPlanningHandler _payoutPlanningHandler = payoutPlanningHandler
        ?? throw new ArgumentNullException(nameof(payoutPlanningHandler));

    private readonly IPayoutExecutionHandler _payoutExecutionHandler = payoutExecutionHandler
        ?? throw new ArgumentNullException(nameof(payoutExecutionHandler));

    private readonly ILogger<DuelSettlementHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<int> PlanDuelPayoutsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting payout planning at {Time}", DateTimeOffset.UtcNow);
            var count = await _payoutPlanningHandler.PlanDuelPayoutsAsync(cancellationToken);
            _logger.LogInformation("Payout planning completed. Settled {Count} duels at {Time}", count, DateTimeOffset.UtcNow);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to plan duel payouts");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> ExecutePendingPayoutsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting payout execution at {Time}", DateTimeOffset.UtcNow);
            var count = await _payoutExecutionHandler.ExecutePendingPayoutsAsync(cancellationToken);
            _logger.LogInformation("Payout execution completed. Processed {Count} payouts at {Time}", count, DateTimeOffset.UtcNow);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute pending payouts");
            throw;
        }
    }
}
