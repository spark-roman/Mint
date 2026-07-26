using Mint.App.Services.System.WinCalculation.Dto;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <summary>
/// Provides calculation for duel results based on duel type.
/// </summary>
public interface IDuelCalculationHandler
{
    /// <summary>
    /// Calculates the result of a duel.
    /// </summary>
    Task<DuelResultDto> CalculateResultAsync(long duelId, long winningOptionId, CancellationToken cancellationToken);
}
