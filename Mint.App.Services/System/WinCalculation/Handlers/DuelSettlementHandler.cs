using System.Collections.ObjectModel;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Settings.Handlers;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.Settings;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database.Entities.Ledger.Transactions.Dto;
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
    IVoteRepository voteRepository,
    IUserStatsRepository userStatsRepository,
    ITransactionRepository transactionRepository,
    IDuelCalculationHandler duelCalculator,
    ISystemSettingHandler systemSettingHandler,
    IDtoMapper<PayoutDto, TransactionCreateDto> payoutMapper,
    IDtoMapper<PayoutDto, UserStatsUpdateDto> userStatsMapper,
    TimeProvider timeProvider,
    ILogger<DuelSettlementHandler> logger) : IDuelSettlementHandler
{
    private readonly IDuelRepository _duelRepository = duelRepository
        ?? throw new ArgumentNullException(nameof(duelRepository));

    private readonly IPayoutRepository _payoutRepository = payoutRepository
        ?? throw new ArgumentNullException(nameof(payoutRepository));

    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));

    private readonly IUserStatsRepository _userStatsRepository = userStatsRepository
        ?? throw new ArgumentNullException(nameof(userStatsRepository));

    private readonly ITransactionRepository _transactionRepository = transactionRepository
        ?? throw new ArgumentNullException(nameof(transactionRepository));

    private readonly IDuelCalculationHandler _duelCalculator = duelCalculator
        ?? throw new ArgumentNullException(nameof(duelCalculator));

    private readonly ISystemSettingHandler _systemSettingHandler = systemSettingHandler
        ?? throw new ArgumentNullException(nameof(systemSettingHandler));
    
    private readonly IDtoMapper<PayoutDto, TransactionCreateDto> _payoutMapper = payoutMapper
        ?? throw new ArgumentNullException(nameof(payoutMapper));

    private readonly IDtoMapper<PayoutDto, UserStatsUpdateDto> _userStatsMapper = userStatsMapper
        ?? throw new ArgumentNullException(nameof(userStatsMapper));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    private readonly ILogger<DuelSettlementHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private async Task PlanDuelPayoutAsync(DuelDto duel, long? winningOptionId, ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken)
    {
        var result = await _duelCalculator.CalculateResultAsync(duel, winningOptionId, votes, cancellationToken);

        foreach (var voteResult in result.VoteResults)
        {
            var payoutCreateDto = new PayoutCreateDto
            {
                VoteId = voteResult.VoteId,
                DuelId = duel.Id,
                AccountId = voteResult.PayoutInstruction.CreditAccountId,
                Amount = voteResult.PayoutInstruction.Amount,
                DebitAccountId = voteResult.PayoutInstruction.DebitAccountId,
                BonusType = voteResult.PayoutInstruction.BonusType,
                Description = voteResult.PayoutInstruction.Description,
                Status = PayoutStatus.Pending,
                ProcessedAt = null,
                TransactionId = null
            };

            await _payoutRepository.CreateAsync(payoutCreateDto, cancellationToken);

            _logger.LogInformation(
                "Payout: {Amount} to account {CreditAccountId}",
                voteResult.PayoutInstruction.Amount,
                voteResult.VoteAccountId);
        }

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

    /// <inheritdoc />
    public async Task<int> ExecutePendingPayoutsAsync(CancellationToken cancellationToken)
    {
        var maxBatchesPerRun = await  _systemSettingHandler.GetIntAsync(SettingKeysConstants.MaxBatchesPerRun, 2000000, cancellationToken);
        var batchSize = await  _systemSettingHandler.GetIntAsync(SettingKeysConstants.BatchSize, 100, cancellationToken);

        var executedCount = 0;

        for (var batch = 0; batch < maxBatchesPerRun; batch++)
        {
            var claimed = await _payoutRepository.ClaimPendingPayoutsAsync(batchSize, cancellationToken);

            if (claimed.Count == 0)
            {
                break;
            }

            foreach (var payout in claimed)
            {
                if (await ExecutePayoutAsync(payout, cancellationToken))
                {
                    executedCount++;
                }
            }

            if (claimed.Count < batchSize)
            {
                break;
            }
        }

        _logger.LogInformation("Payout execution finished. Executed {Count} payouts", executedCount);

        return executedCount;
    }

    private readonly List<BonusType> _bonusTypes = [ BonusType.Bet, BonusType.Refund ];

    private async Task<bool> ExecutePayoutAsync(PayoutDto payout, CancellationToken cancellationToken)
    {
        var transaction = _payoutMapper.Map(payout);
        long? transactionId;

        try
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromSeconds(30)
            };

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                transactionOptions,
                TransactionScopeAsyncFlowOption.Enabled);
            
            var userStats = await _userStatsRepository.GetStatsByAccountIdAsync(payout.CreditAccountId, cancellationToken);

            if (userStats is null)
            {
                throw new InvalidOperationException($"User stats not found for account {payout.CreditAccountId}");
            }

            var statsUpdateDto = _userStatsMapper.Map(payout, userStats);

            await _userStatsRepository.UpdateStatsByAccountIdAsync(payout.CreditAccountId, statsUpdateDto, cancellationToken);

            transactionId = _bonusTypes.Contains(payout.BonusType)
                ? await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken)
                : 0;

            var completed = await _payoutRepository.TryCompleteAsync(
                payout.Id,
                transactionId,
                _timeProvider.GetUtcNow(),
                cancellationToken);

            if (!completed)
            {
                _logger.LogError(
                    "Payout {PayoutId} is not in processing state anymore, transaction {TransactionId} is already booked",
                    payout.Id,
                    transactionId);

                return false;
            }

            _logger.LogInformation(
                "Payout {PayoutId} completed: {Amount} to account {AccountId}, transaction {TransactionId}",
                payout.Id,
                payout.Amount,
                payout.CreditAccountId,
                transactionId);

            scope.Complete();

            return true;
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "Payout {PayoutId} execution was cancelled, it stays in processing",
                payout.Id);

            throw;
        }
    }
}
