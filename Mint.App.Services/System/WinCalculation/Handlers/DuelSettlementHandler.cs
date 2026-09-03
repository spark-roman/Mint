using Microsoft.Extensions.Logging;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.System.Payouts.Dto;
using Mint.Database.Entities.System.Payouts.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <inheritdoc cref="IDuelSettlementHandler"/>
public sealed class DuelSettlementHandler(
    IDuelRepository duelRepository,
    IPayoutRepository payoutRepository,
    ITransactionRepository transactionRepository,
    IUserStatsRepository userStatsRepository,
    IDuelCalculationHandler duelCalculator,
    TimeProvider timeProvider,
    ILogger<DuelSettlementHandler> logger) : IDuelSettlementHandler
{
    private readonly IDuelRepository _duelRepository = duelRepository
        ?? throw new ArgumentNullException(nameof(duelRepository));

    private readonly IPayoutRepository _payoutRepository = payoutRepository
        ?? throw new ArgumentNullException(nameof(payoutRepository));

    private readonly ITransactionRepository _transactionRepository = transactionRepository
        ?? throw new ArgumentNullException(nameof(transactionRepository));

    private readonly IUserStatsRepository _userStatsRepository = userStatsRepository
        ?? throw new ArgumentNullException(nameof(userStatsRepository));

    private readonly IDuelCalculationHandler _duelCalculator = duelCalculator
        ?? throw new ArgumentNullException(nameof(duelCalculator));

    private readonly TimeProvider _timeProvider = timeProvider
        ?? throw new ArgumentNullException(nameof(timeProvider));

    private readonly ILogger<DuelSettlementHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<int> SettleExpiredDuelsAsync(CancellationToken cancellationToken)
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
            await SettleDuelByVotesAsync(duel, cancellationToken);
            settledCount++;
        }

        return settledCount;
    }

    /// <inheritdoc />
    public async Task SettleDuelAsync(long duelId, CancellationToken cancellationToken)
    {
        var duel = await _duelRepository.GetDuelByIdAsync(duelId, cancellationToken);

        if (duel == null)
        {
            throw new InvalidOperationException($"Duel {duelId} not found");
        }

        if (duel.Status == DuelStatus.Closed)
        {
            throw new InvalidOperationException($"Duel {duelId} is already closed");
        }
        
        await SettleDuelByVotesAsync(duel, cancellationToken);
    }

    private async Task SettleDuelByVotesAsync(DuelDto duel, CancellationToken cancellationToken)
    {
        var winningOptionId = await _duelCalculator.CalculateWinningOptionIdAsync(duel.Id, DuelType.OpinionMatch, cancellationToken);
        
        if (winningOptionId is null)
        {
            _logger.LogWarning("No winners found for duel {DuelId}, closing without settlement", duel.Id);
        }
        else
        {
            await ProcessSettlementAsync(duel.Id, winningOptionId.Value, cancellationToken);
            
            _logger.LogInformation(
                "Duel {DuelId} settled with winning option {WinningOptionId} by majority vote",
                duel.Id,
                winningOptionId);
        }

        await _duelRepository.CloseDuelAsync(duel.Id, cancellationToken);
    }

    private async Task ProcessSettlementAsync(long duelId, long winningOptionId, CancellationToken cancellationToken)
    {
        var result = await _duelCalculator.CalculateResultAsync(duelId, winningOptionId, cancellationToken);

        foreach (var voteResult in result.VoteResults)
        {
            if (voteResult.PayoutInstruction is null)
            {
                var userStats = await _userStatsRepository.GetStatsByAccountIdAsync(voteResult.VoteAccountId, cancellationToken);

                if (userStats is null)
                {
                    throw new InvalidOperationException($"User stats not found for account {voteResult.VoteAccountId}");
                }

                var statsUpdateDto = new UserStatsUpdateDto
                {
                    RankPoints = userStats.RankPoints,
                    TotalWins = userStats.TotalWins,
                    TotalLosses = userStats.TotalLosses + 1,
                    ReferralCount = userStats.ReferralCount
                };

                await _userStatsRepository.UpdateStatsByAccountIdAsync(voteResult.VoteAccountId, statsUpdateDto, cancellationToken);
            }
            else
            {
                var transactionId = await _transactionRepository.CreateTransactionAsync(voteResult.PayoutInstruction, cancellationToken);

                var userStats = await _userStatsRepository.GetStatsByAccountIdAsync(voteResult.VoteAccountId, cancellationToken);

                if (userStats is null)
                {
                    throw new InvalidOperationException($"User stats not found for account {voteResult.VoteAccountId}");
                }

                var statsUpdateDto = new UserStatsUpdateDto
                {
                    RankPoints = userStats.RankPoints + voteResult.PayoutInstruction.Amount,
                    TotalWins = userStats.TotalWins + 1,
                    TotalLosses = userStats.TotalLosses,
                    ReferralCount = userStats.ReferralCount
                };

                await _userStatsRepository.UpdateStatsByAccountIdAsync(voteResult.VoteAccountId, statsUpdateDto, cancellationToken);

                var payoutCreateDto = new PayoutCreateDto
                {
                    VoteId = voteResult.VoteId,
                    DuelId = duelId,
                    AccountId = voteResult.PayoutInstruction.CreditAccountId,
                    Amount = voteResult.PayoutInstruction.Amount,
                    ProcessedAt = _timeProvider.GetUtcNow(),
                    TransactionId = transactionId
                };

                await _payoutRepository.CreateAsync(payoutCreateDto, cancellationToken);

                _logger.LogDebug(
                    "Payout: {Amount} to account {CreditAccountId}",
                    voteResult.PayoutInstruction.Amount,
                    voteResult.VoteAccountId);
            }
        }

        var (totalPayout, payoutCount)  = result.VoteResults
            .Where(v => v.PayoutInstruction is not null)
            .Aggregate(
                (Total: 0m, Count: 0),
                (acc, v) => (
                    Total: acc.Total + v.PayoutInstruction!.Amount,
                    Count: acc.Count + 1
                )
            );

        if (payoutCount == 0)
        {
            _logger.LogWarning("No payout instructions for duel {DuelId}", duelId);
        }
        else
        {
            _logger.LogInformation(
                "Duel {DuelId} settled: {PayoutCount} payouts, total {TotalPayout}",
                duelId,
                payoutCount,
                totalPayout);
        }
    }
}
