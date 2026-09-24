using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.System.Payouts.Dto;
using Mint.Database.Entities.System.Payouts.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <inheritdoc cref="IDuelSettlementHandler"/>
public sealed class DuelSettlementHandler(
    IDuelRepository duelRepository,
    IPayoutRepository payoutRepository,
    ITransactionRepository transactionRepository,
    IUserStatsRepository userStatsRepository,
    IVoteRepository voteRepository,
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

    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));

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
            try
            {
                await SettleDuelByVotesAsync(duel, cancellationToken);
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
        var currentDuel = await _duelRepository.GetDuelByIdAsync(duel.Id, cancellationToken);

        if (currentDuel is null || currentDuel.Status == DuelStatus.Closed)
        {
            _logger.LogWarning("Duel {DuelId} is already closed or missing, skipping settlement", duel.Id);
            return;
        }

        var votes = await _voteRepository.GetVotesByDuelIdAsync(duel.Id, cancellationToken);

        var winningOptionId = await _duelCalculator.CalculateWinningOptionIdAsync(duel.DuelType, votes.AsReadOnly(), cancellationToken);

        await ProcessSettlementAsync(duel, winningOptionId, votes.AsReadOnly(), cancellationToken);

        _logger.LogInformation(
            "Duel {DuelId} settled with winning option {WinningOptionId} by majority vote",
            duel.Id,
            winningOptionId);

        await _duelRepository.CloseDuelAsync(duel.Id, cancellationToken);
    }

    private async Task ProcessSettlementAsync(DuelDto duel, long? winningOptionId, ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken)
    {
        var result = await _duelCalculator.CalculateResultAsync(duel, winningOptionId, votes, cancellationToken);

        foreach (var voteResult in result.VoteResults)
        {
            if (voteResult.PayoutInstruction is null)
            {
                var userStats = await _userStatsRepository.GetStatsByAccountIdAsync(voteResult.VoteAccountId, cancellationToken);

                if (userStats is null)
                {
                    throw new InvalidOperationException($"User stats not found for account {voteResult.VoteAccountId}");
                }

                var loseStatsDto = new UserStatsUpdateDto
                {
                    RankPoints = userStats.RankPoints,
                    TotalWins = userStats.TotalWins,
                    TotalLosses = userStats.TotalLosses + 1,
                    TotalDraws = userStats.TotalDraws,
                    ReferralCount = userStats.ReferralCount
                };

                await _userStatsRepository.UpdateStatsByAccountIdAsync(voteResult.VoteAccountId, loseStatsDto, cancellationToken);

                _logger.LogInformation("Lose account id: {CreditAccountId}", voteResult.VoteAccountId);
            }
            else
            {
                var transactionId = await _transactionRepository.CreateTransactionAsync(voteResult.PayoutInstruction, cancellationToken);

                var userStats = await _userStatsRepository.GetStatsByAccountIdAsync(voteResult.VoteAccountId, cancellationToken);

                if (userStats is null)
                {
                    throw new InvalidOperationException($"User stats not found for account {voteResult.VoteAccountId}");
                }

                var statsUpdateDto = voteResult.PayoutInstruction.BonusType switch
                {
                    BonusType.Bet => new UserStatsUpdateDto
                    {
                        RankPoints = userStats.RankPoints + voteResult.PayoutInstruction.Amount,
                        TotalWins = userStats.TotalWins + 1,
                        TotalLosses = userStats.TotalLosses,
                        TotalDraws = userStats.TotalDraws,
                        ReferralCount = userStats.ReferralCount
                    },
                    BonusType.Refund => new UserStatsUpdateDto
                    {
                        RankPoints = userStats.RankPoints,
                        TotalWins = userStats.TotalWins,
                        TotalLosses = userStats.TotalLosses,
                        TotalDraws = userStats.TotalDraws + 1,
                        ReferralCount = userStats.ReferralCount
                    },
                    _ => throw new InvalidOperationException("Invalid bonus type")
                };

                await _userStatsRepository.UpdateStatsByAccountIdAsync(voteResult.VoteAccountId, statsUpdateDto, cancellationToken);

                var payoutCreateDto = new PayoutCreateDto
                {
                    VoteId = voteResult.VoteId,
                    DuelId = duel.Id,
                    AccountId = voteResult.PayoutInstruction.CreditAccountId,
                    Amount = voteResult.PayoutInstruction.Amount,
                    ProcessedAt = _timeProvider.GetUtcNow(),
                    TransactionId = transactionId
                };

                await _payoutRepository.CreateAsync(payoutCreateDto, cancellationToken);

                _logger.LogInformation(
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
