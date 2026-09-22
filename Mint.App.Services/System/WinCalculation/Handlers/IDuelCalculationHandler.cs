using System.Collections.ObjectModel;
using Mint.App.Services.System.WinCalculation.Dto;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Dto;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <summary>
/// Provides calculation for duel results based on duel type.
/// </summary>
public interface IDuelCalculationHandler
{
    /// <summary>
    /// Calculates the result of a duel.
    /// </summary>
    /// <param name="duel">Duel.</param>
    /// <param name="winningOptionId">Winning option id.</param>
    /// <param name="votes">Duel votes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Duel calculation result.</returns>
    Task<DuelResultDto> CalculateResultAsync(DuelDto duel, long? winningOptionId, ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken);

    /// <summary>
    /// Calculates the winning option id for a duel.
    /// </summary>
    /// <param name="duelType">Duel type.</param>
    /// <param name="votes">Duel votes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Winning option id.</returns>
    Task<long?> CalculateWinningOptionIdAsync(DuelType duelType, ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken);
}
