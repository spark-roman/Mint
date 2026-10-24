namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <summary>
/// Handles planning of payouts for expired duels.
/// </summary>
public interface IPayoutPlanningHandler
{
    /// <summary>
    /// Plans payouts for all expired duels.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of duels settled</returns>
    Task<int> PlanDuelPayoutsAsync(CancellationToken cancellationToken);
}
