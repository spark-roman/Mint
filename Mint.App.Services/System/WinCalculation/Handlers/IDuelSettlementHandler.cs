namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <summary>
/// Handles settlement of duel results.
/// </summary>
public interface IDuelSettlementHandler
{
    /// <summary>
    /// Plans payouts for duels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of payouts set</returns>
    Task<int> PlanDuelPayoutsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Executes pending payouts for duels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of payouts set</returns>
    Task<int> ExecutePendingPayoutsAsync(CancellationToken cancellationToken);
}
