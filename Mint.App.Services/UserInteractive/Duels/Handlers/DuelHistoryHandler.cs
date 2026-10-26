using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;

namespace Mint.App.Services.UserInteractive.Duels.Handlers;

/// <summary>
/// Handler for duel history
/// </summary>
/// <param name="userStatsRepository">User stats repository</param>
public sealed class DuelHistoryHandler(
    IUserStatsRepository userStatsRepository) : IDuelHistoryHandler
{
    private readonly IUserStatsRepository _userStatsRepository = userStatsRepository
        ?? throw new ArgumentNullException(nameof(userStatsRepository));

    /// <inheritdoc/>
    public Task<List<DuelHistoryDto>> GetDuelHistoryAsync(long accountId, CancellationToken cancellationToken)
    {
        return _userStatsRepository.GetDuelHistoryAsync(accountId, cancellationToken);
    }
}
