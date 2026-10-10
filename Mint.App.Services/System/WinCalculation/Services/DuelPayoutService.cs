using Microsoft.Extensions.Logging;
using Mint.App.Services.System.WinCalculation.Handlers;

namespace Mint.App.Services.System.WinCalculation.Services;

/// <inheritdoc cref="IDuelPayoutService"/>
public sealed class DuelPayoutService(
    IDuelSettlementHandler settlementHandler,
    ILogger<DuelPayoutService> logger) : IDuelPayoutService
{
    private readonly IDuelSettlementHandler _settlementHandler = settlementHandler ?? throw new ArgumentNullException(nameof(settlementHandler));
    
    private readonly ILogger<DuelPayoutService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<int> ProcessExpiredDuelsPayoutsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting payouts plan generation at {Time}", DateTimeOffset.UtcNow);

            var settledCount = await _settlementHandler.PlanDuelPayoutsAsync(cancellationToken);

            _logger.LogInformation(
                "Completed duel payouts processing. Settled {Count} duels at {Time}",
                settledCount,
                DateTimeOffset.UtcNow);

            return settledCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process duel payouts");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> ExecutePendingPayoutsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting payouts plan execution at {Time}", DateTimeOffset.UtcNow);

            var processedCount = await _settlementHandler.ExecutePendingPayoutsAsync(cancellationToken);

            _logger.LogInformation(
                "Completed pending payouts processing. Processed {Count} duels at {Time}",
                processedCount,
                DateTimeOffset.UtcNow);

            return processedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute pending payouts");
            throw;
        }
    }
}

