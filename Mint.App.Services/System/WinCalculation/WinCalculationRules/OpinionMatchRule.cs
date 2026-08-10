using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;

namespace Mint.App.Services.System.WinCalculation.WinCalculationRules;

/// <summary>
/// Opinion match rule.
/// </summary>
public class OpinionMatchRule(IVoteRepository voteRepository) : IWinCalculationRule
{
    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));

    /// <inheritdoc />
    public async Task<List<long>> CalculateAsync(long duelId, CancellationToken cancellationToken)
    {
        var votes = await _voteRepository.GetVotesByDuelIdAsync(duelId, cancellationToken);

        if (votes is null || votes.Count == 0)
        {
            return [];
        }

        var maxVoteCount = votes
            .GroupBy(v => v.ChosenOptionId)
            .Max(g => g.Count());

        var winningOptionIds = votes
            .GroupBy(v => v.ChosenOptionId)
            .Where(g => g.Count() == maxVoteCount)
            .Select(g => g.Key)
            .ToList();

        return winningOptionIds;
    }

    /// <inheritdoc />
    public Task<bool> IsMatchedAsync(DuelType duelType) => Task.FromResult(duelType == DuelType.OpinionMatch);
}
