using System.Collections.ObjectModel;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes.Dto;

namespace Mint.App.Services.System.WinCalculation.WinCalculationRules;

/// <summary>
/// Rule for calculating winning option id
/// </summary>
public interface IWinCalculationRule
{
    /// <summary>
    /// Calculate winning option id
    /// </summary>
    /// <param name="votes">Duel votes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Winning option id</returns>
    Task<long?> CalculateAsync(ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken);

    /// <summary>
    /// Check if rule matches given duel type
    /// </summary>
    /// <param name="duelType">Duel type</param>
    /// <returns>Is matched</returns>
    Task<bool> IsMatchedAsync(DuelType duelType);
}
