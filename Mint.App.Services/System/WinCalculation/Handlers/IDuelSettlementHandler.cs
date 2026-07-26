namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <summary>
/// Handles settlement of duel results.
/// </summary>
public interface IDuelSettlementHandler
{
    /// <summary>
    /// Settles all active duels that have expired.
    /// </summary>
    Task<int> SettleExpiredDuelsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Settles a specific duel by ID.
    /// </summary>
    Task SettleDuelAsync(long duelId, CancellationToken cancellationToken);
}
