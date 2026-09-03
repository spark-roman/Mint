using Mint.Common.Contracts.UserInteractive.Duels;

namespace Mint.App.Services.System.WinCalculation.WinCalculationRules;

/// <summary>
/// Rule for calculating winning option id
/// </summary>
public interface IWinCalculationRule
{
    /// <summary>
    /// Calculate winning option id
    /// </summary>
    /// <param name="duelId">Duel id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Winning option id</returns>
    Task<long?> CalculateAsync(long duelId, CancellationToken cancellationToken);

    /// <summary>
    /// Check if rule matches given duel type
    /// </summary>
    /// <param name="duelType">Duel type</param>
    /// <returns>Is matched</returns>
    Task<bool> IsMatchedAsync(DuelType duelType);
}
