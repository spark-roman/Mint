using Mint.Common.Contracts.UserInteractive.Duels;

namespace Mint.App.Services.UserInteractive.Duels.Handlers;

/// <summary>
/// Handler for duel history
/// </summary>
public interface IDuelHistoryHandler
{
    /// <summary>
    /// Gets the history of all duels the user voted in
    /// </summary>
    /// <param name="accountId">Account ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of duel history entries</returns>
    Task<List<DuelHistoryDto>> GetDuelHistoryAsync(long accountId, CancellationToken cancellationToken);
}
