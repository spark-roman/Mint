using Mint.App.Services.UserInteractive.Bonuses.Dto;
using Mint.App.Services.UserInteractive.Bonuses.Rules;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Database.Entities.Ledger.Transactions.Dto;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;

namespace Mint.App.Services.UserInteractive.Bonuses.Handlers;

/// <inheritdoc/>
public class BonusCalculationHandler(
    IBonusValidator bonusValidator,
    ITransactionRepository transactionRepository,
    IUserBonusStatsRepository bonusStatsRepository,
    TimeProvider timeProvider) : IBonusCalculationHandler
{
    private readonly IBonusValidator _bonusValidator = bonusValidator ?? throw new ArgumentNullException(nameof(bonusValidator));

    private readonly ITransactionRepository _transactionRepository = transactionRepository
        ?? throw new ArgumentNullException(nameof(transactionRepository));
    
    private readonly IUserBonusStatsRepository _bonusStatsRepository = bonusStatsRepository
        ?? throw new ArgumentNullException(nameof(bonusStatsRepository));
    private readonly TimeProvider _timeProvider = timeProvider
        ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc/>
    public async Task<BonusResultDto> ApplyStartBonusAsync(long externalUserId, byte systemType, long accountId, CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();
        var bonusStats = await _bonusStatsRepository.GetStatsByUserIdAsync(externalUserId, systemType, cancellationToken);

        if (!await _bonusValidator.CanApplyStartBonus(bonusStats, cancellationToken))
        {
            return new BonusResultDto
            {
                Success = false,
                AlreadyApplied = true,
                Message = "Bonus already applied"
            };
        }

        const decimal startBonusAmount = 1000.00m;

        var transaction = new TransactionCreateDto
        {
            DebitAccountId = 1,
            CreditAccountId = accountId,
            Amount = startBonusAmount,
            Description = "Стартовый бонус",
            BonusType = BonusType.Start,
            CreatedAt = now
        };

        await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken);

        if (bonusStats == null)
        {
            var newStats = new UserBonusStatsCreateDto
            {
                ExternalUserId = externalUserId,
                IsStartBonusClaimed = true,
                TotalStartBonusesClaimed = startBonusAmount,
                StartBonusClaimedAt = now,
                NextDailyAvailableAt = now
            };

            await _bonusStatsRepository.CreateStatsAsync(newStats, cancellationToken);
        }
        else
        {
            var updateStats = new UserBonusStatsUpdateDto
            {
                ExternalUserId = externalUserId,
                IsStartBonusClaimed = true,
                TotalStartBonusesClaimed = bonusStats.TotalStartBonusesClaimed + startBonusAmount,
                StartBonusClaimedAt = now,
                CurrentDailyStreak = bonusStats.CurrentDailyStreak,
                TotalStreakBonusesClaimed = bonusStats.TotalStreakBonusesClaimed,
                LastDailyClaimedAt = bonusStats.LastDailyClaimedAt,
                NextDailyAvailableAt = bonusStats.NextDailyAvailableAt ?? now,
                TotalDailyBonusesClaimed = bonusStats.TotalDailyBonusesClaimed,
                TotalReferralBonusesClaimed = bonusStats.TotalReferralBonusesClaimed,
                TotalRankBonusClaimed = bonusStats.TotalRankBonusClaimed,
                LastStreakClaimedAt = bonusStats.LastStreakClaimedAt,
                LastRankBonusClaimedAt = bonusStats.LastRankBonusClaimedAt
            };

            await _bonusStatsRepository.UpdateStatsAsync(updateStats, cancellationToken);
        }

        return new BonusResultDto
        {
            Message = "🎉 Стартовый бонус получен! +1000 🪙",
            Amount = startBonusAmount,
            Success = true,
        };
    }

    /// <inheritdoc/>
    public async Task<BonusResultDto> ApplyDailyBonusAsync(long externalUserId, byte systemType, long accountId, CancellationToken cancellationToken)
    {
        var now = _timeProvider.GetUtcNow();
        var bonusStats = await _bonusStatsRepository.GetStatsByUserIdAsync(externalUserId, systemType, cancellationToken);

        if (bonusStats == null)
        {
            var newStats = new UserBonusStatsCreateDto
            {
                ExternalUserId = externalUserId
            };

            await _bonusStatsRepository.CreateStatsAsync(newStats, cancellationToken);

            bonusStats = await _bonusStatsRepository.GetStatsByUserIdAsync(externalUserId, systemType, cancellationToken);
        }

        if (!await _bonusValidator.CanApplyDailyBonus(bonusStats, cancellationToken))
        {
            return new BonusResultDto
            {
                Success = false,
                Message = $"Бонус будет доступен после {bonusStats!.NextDailyAvailableAt:HH:mm}"
            };
        }

        const decimal dailyBonusAmount = 100m;
        const decimal streakBonusAmount = 1000m;

        var dailyTransaction = new TransactionCreateDto
        {
            DebitAccountId = 1,
            CreditAccountId = accountId,
            Amount = dailyBonusAmount,
            Description = $"Ежедневный бонус (День {bonusStats!.CurrentDailyStreak + 1})",
            BonusType = BonusType.Daily,
            CreatedAt = now
        };

        await _transactionRepository.CreateTransactionAsync(dailyTransaction, cancellationToken);

        decimal totalBonus = dailyBonusAmount;
        var isStreakBonusApplied = false;

        var newStreak = bonusStats.CurrentDailyStreak + 1;
        if (newStreak == 7)
        {
            var streakTransaction = new TransactionCreateDto
            {
                DebitAccountId = 1,
                CreditAccountId = accountId,
                Amount = streakBonusAmount,
                Description = "🔥 Стрик-бонус за 7 дней подряд!",
                BonusType = BonusType.Streak,
                CreatedAt = now
            };

            await _transactionRepository.CreateTransactionAsync(streakTransaction, cancellationToken);
            totalBonus += streakBonusAmount;
            isStreakBonusApplied = true;
        }

        var updateDto = new UserBonusStatsUpdateDto
        {
            ExternalUserId = externalUserId,
            IsStartBonusClaimed = bonusStats.IsStartBonusClaimed,
            CurrentDailyStreak = newStreak == 7 ? 0 : newStreak,
            LastDailyClaimedAt = now,
            NextDailyAvailableAt = now.AddHours(24),
            TotalDailyBonusesClaimed = bonusStats.TotalDailyBonusesClaimed + dailyBonusAmount,
            TotalStreakBonusesClaimed = bonusStats.TotalStreakBonusesClaimed + (isStreakBonusApplied ? streakBonusAmount : 0),
            LastStreakClaimedAt = isStreakBonusApplied ? now : bonusStats.LastStreakClaimedAt,
            TotalStartBonusesClaimed = bonusStats.TotalStartBonusesClaimed,
            TotalReferralBonusesClaimed = bonusStats.TotalReferralBonusesClaimed,
            TotalRankBonusClaimed = bonusStats.TotalRankBonusClaimed,
            LastRankBonusClaimedAt = bonusStats.LastRankBonusClaimedAt
        };

        await _bonusStatsRepository.UpdateStatsAsync(updateDto, cancellationToken);

        var message = isStreakBonusApplied
            ? $"🔥 **Стрик 7 дней!**\n\nПолучено: {totalBonus:N0} 🪙\n(100 ежедневных + 1000 за стрик)"
            : $"✅ **Ежедневный бонус получен!**\n\n{totalBonus:N0} 🪙 (День {newStreak})";

        return new BonusResultDto
        {
            Success = true,
            Message = message,
            Amount = totalBonus,
            IsStreakBonusApplied = isStreakBonusApplied,
        };
    }
}
