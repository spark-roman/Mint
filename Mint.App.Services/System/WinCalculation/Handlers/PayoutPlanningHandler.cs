using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database.Entities.System.Payouts.Dto;
using Mint.Database.Entities.System.Payouts.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <inheritdoc cref="IPayoutPlanningHandler"/>
public sealed class PayoutPlanningHandler(
    IDuelRepository duelRepository,
    IPayoutRepository payoutRepository,
    IVoteRepository voteRepository,
    IDuelCalculationHandler duelCalculator,
    ILogger<PayoutPlanningHandler> logger) : IPayoutPlanningHandler
{
    private readonly IDuelRepository _duelRepository = duelRepository
        ?? throw new ArgumentNullException(nameof(duelRepository));

    private readonly IPayoutRepository _payoutRepository = payoutRepository
        ?? throw new ArgumentNullException(nameof(payoutRepository));

    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));

    private readonly IDuelCalculationHandler _duelCalculator = duelCalculator
        ?? throw new ArgumentNullException(nameof(duelCalculator));

    private readonly ILogger<PayoutPlanningHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<int> PlanDuelPayoutsAsync(CancellationToken cancellationToken)
    {
        var expiredDuels = await _duelRepository.GetActiveDuelsForCloseAsync(cancellationToken);

        if (expiredDuels is null || expiredDuels.Count == 0)
        {
            _logger.LogInformation("No expired duels to settle");
            return 0;
        }

        _logger.LogInformation("Settling {Count} expired duels", expiredDuels.Count);

        var settledCount = 0;
        
        foreach (var duel in expiredDuels)
        {
            try
            {
                var votes = await _voteRepository.GetVotesByDuelIdAsync(duel.Id, cancellationToken);
                var winningOptionId = await _duelCalculator.CalculateWinningOptionIdAsync(duel.DuelType, votes.AsReadOnly(), cancellationToken);

                await PlanDuelPayoutAsync(duel, winningOptionId, votes.AsReadOnly(), cancellationToken);

                _logger.LogInformation(
                    "Duel {DuelId} settled with winning option {WinningOptionId} by majority vote",
                    duel.Id,
                    winningOptionId);

                await _duelRepository.CloseDuelAsync(duel.Id, cancellationToken);

                settledCount++;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to settle duel {DuelId}", duel.Id);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to settle duel {DuelId}", duel.Id);
            }
        }

        return settledCount;
    }

    private async Task PlanDuelPayoutAsync(DuelDto duel, long? winningOptionId, ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken)
    {
        var result = await _duelCalculator.CalculateResultAsync(duel, winningOptionId, votes, cancellationToken);

        var payouts = result.VoteResults.Select(vr => new PayoutCreateDto
        {
            VoteId = vr.VoteId,
            DuelId = duel.Id,
            AccountId = vr.PayoutInstruction.CreditAccountId,
            Amount = vr.PayoutInstruction.Amount,
            DebitAccountId = vr.PayoutInstruction.DebitAccountId,
            BonusType = vr.PayoutInstruction.BonusType,
            Description = vr.PayoutInstruction.Description,
            Status = PayoutStatus.Pending,
            ProcessedAt = null,
            TransactionId = null
        }).ToList();

        await _payoutRepository.CreateManyAsync(new ReadOnlyCollection<PayoutCreateDto>(payouts), cancellationToken);

        var (totalPayout, payoutCount)  = result.VoteResults
            .Where(v => v.PayoutInstruction is not null)
            .Aggregate(
                (Total: 0m, Count: 0),
                (acc, v) => (
                    Total: acc.Total + v.PayoutInstruction.Amount,
                    Count: acc.Count + 1
                )
            );

        if (payoutCount == 0)
        {
            _logger.LogWarning("No payout instructions for duel {DuelId}", duel.Id);
        }
        else
        {
            _logger.LogInformation(
                "Duel {DuelId} settled: {PayoutCount} payouts, total {TotalPayout}",
                duel.Id,
                payoutCount,
                totalPayout);
        }
    }
}
