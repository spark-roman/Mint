using Mint.App.Services.System.WinCalculation.Dto;
using Mint.Common.Contracts.UserInteractive.Duels;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <summary>
/// Provides calculation for duel results based on duel type.
/// </summary>
public interface IDuelCalculationHandler
{
    /// <summary>
    /// Calculates the result of a duel.
    /// </summary>
    /// <param name="duelId">Duel id/</param>
    /// <param name="winningOptionId">Winning option id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Duel calculation result.</returns>
    Task<DuelResultDto> CalculateResultAsync(long duelId, long winningOptionId, CancellationToken cancellationToken);

    /// <summary>
    /// Calculates the winning option id for a duel.
    /// </summary>
    /// <param name="duelId">Duel id.</param>
    /// <param name="duelType">Duel type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Winning option id.</returns>
    Task<long?> CalculateWinningOptionIdAsync(long duelId, DuelType duelType, CancellationToken cancellationToken);
}
