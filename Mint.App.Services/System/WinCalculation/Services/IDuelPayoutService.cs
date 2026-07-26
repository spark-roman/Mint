namespace Mint.App.Services.System.WinCalculation.Services;

/// <summary>
/// Service for processing duel payouts.
/// </summary>
public interface IDuelPayoutService
{
    /// <summary>
    /// Processes all expired duels and distributes payouts to winners.
    /// </summary>
    Task<int> ProcessExpiredDuelsPayoutsAsync(CancellationToken cancellationToken);
}
