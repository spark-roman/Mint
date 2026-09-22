using System.Collections.ObjectModel;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes.Dto;

namespace Mint.App.Services.System.WinCalculation.WinCalculationRules;

/// <summary>
/// Opinion match rule.
/// </summary>
public class OpinionMatchRule: IWinCalculationRule
{
    /// <inheritdoc />
    public async Task<long?> CalculateAsync(ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken)
    {
        if (votes is null || votes.Count == 0)
        {
            return null;
        }

        var maxVoteCount = votes
            .GroupBy(v => v.ChosenOptionId)
            .Max(g => g.Count());

        var winningOptionIds = votes
            .GroupBy(v => v.ChosenOptionId)
            .Where(g => g.Count() == maxVoteCount)
            .Select(g => g.Key)
            .ToList();

        var result = winningOptionIds.Count == 1 ? winningOptionIds.First() : (long?)null;

        return await Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<bool> IsMatchedAsync(DuelType duelType) => Task.FromResult(duelType == DuelType.OpinionMatch);
}
