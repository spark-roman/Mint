using System.Transactions;
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
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <inheritdoc cref="IPayoutExecutionHandler"/>
public sealed class PayoutExecutionHandler(
    IPayoutRepository payoutRepository,
    IUserStatsRepository userStatsRepository,
    ITransactionRepository transactionRepository,
    IDtoMapper<PayoutDto, TransactionCreateDto> payoutMapper,
    IDtoMapper<PayoutDto, UserStatsUpdateDto> userStatsMapper,
    TimeProvider timeProvider,
    ISystemSettingHandler systemSettingHandler,
    ILogger<PayoutExecutionHandler> logger) : IPayoutExecutionHandler
{
    private readonly IPayoutRepository _payoutRepository = payoutRepository
        ?? throw new ArgumentNullException(nameof(payoutRepository));

    private readonly IUserStatsRepository _userStatsRepository = userStatsRepository
        ?? throw new ArgumentNullException(nameof(userStatsRepository));

    private readonly ITransactionRepository _transactionRepository = transactionRepository
        ?? throw new ArgumentNullException(nameof(transactionRepository));

    private readonly IDtoMapper<PayoutDto, TransactionCreateDto> _payoutMapper = payoutMapper
        ?? throw new ArgumentNullException(nameof(payoutMapper));

    private readonly IDtoMapper<PayoutDto, UserStatsUpdateDto> _userStatsMapper = userStatsMapper
        ?? throw new ArgumentNullException(nameof(userStatsMapper));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    private readonly ISystemSettingHandler _systemSettingHandler = systemSettingHandler
        ?? throw new ArgumentNullException(nameof(systemSettingHandler));

    private readonly ILogger<PayoutExecutionHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private static readonly BonusType[] _bonusTypes = [BonusType.Payout, BonusType.Refund];

    /// <inheritdoc />
    public async Task<int> ExecutePendingPayoutsAsync(CancellationToken cancellationToken)
    {
        var maxBatchesPerRun = await _systemSettingHandler.GetIntAsync(SettingKeysConstants.MaxBatchesPerRun, 1000, cancellationToken);
        var batchSize = await _systemSettingHandler.GetIntAsync(SettingKeysConstants.BatchSize, 100, cancellationToken);

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
                : null;

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
