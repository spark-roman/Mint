using AdvApplication.Auth.Users;
using Mint.App.Services.UserInteractive.Bonuses.Handlers;
using Mint.App.Services.UserInteractive.Bonuses.Rules;
using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Accounts.Dto;
using Mint.Database.Entities.Ledger.Transactions.Dto;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Bonuses.Dto;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.Database.Entities.Users.Dto;

namespace Mint.App.Services.UserInteractive.Profiles.Handlers;

/// <inheritdoc/>
public class UserProfilesHandler(
    IBonusCalculationHandler bonusCalculationHandler,
    IRankConfigRepository rankConfigRepository,
    IUserStatsRepository statsRepository,
    IAccountRepository accountRepository,
    IUserBonusStatsRepository bonusStatsRepository,
    IUserRepository userRepository,
    ITransactionRepository transactionRepository,
    TimeProvider timeProvider,
    IBonusValidator bonusValidator) : IUserProfilesHandler
{
    private readonly IBonusCalculationHandler _bonusCalculationHandler = bonusCalculationHandler
        ?? throw new ArgumentNullException(nameof(bonusCalculationHandler));

    private readonly IRankConfigRepository _rankConfigRepository = rankConfigRepository
        ?? throw new ArgumentNullException(nameof(rankConfigRepository));

    private readonly IUserStatsRepository _statsRepository = statsRepository
        ?? throw new ArgumentNullException(nameof(statsRepository));

    private readonly IAccountRepository _accountRepository = accountRepository
        ?? throw new ArgumentNullException(nameof(accountRepository));

    private readonly IUserBonusStatsRepository _bonusStatsRepository = bonusStatsRepository
        ?? throw new ArgumentNullException(nameof(bonusStatsRepository));

    private readonly IUserRepository _userRepository = userRepository
        ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    
    private readonly IBonusValidator _bonusValidator = bonusValidator ?? throw new ArgumentNullException(nameof(bonusValidator));

    private readonly ITransactionRepository _transactionRepository = transactionRepository
        ?? throw new ArgumentNullException(nameof(transactionRepository));

        /// <inheritdoc />
    public async Task<UserDto> InitializeUserAsync(UserCreateDto userCreateDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userCreateDto);

        await _userRepository.CreateOrUpdateUserAsync(userCreateDto, cancellationToken);

        var accountCreateDto = new AccountCreateDto
        {
            ExternalUserId = userCreateDto.ExternalUserId,
            SystemType = userCreateDto.SystemType,
            Balance = 0.0m,
            CreatedAt = _timeProvider.GetUtcNow(),
            Status = AccountStatus.Active
        };

        var existingAccount = await _accountRepository.GetAccountByExternalUserIdAsync(
            accountCreateDto.ExternalUserId,
            accountCreateDto.SystemType,
            cancellationToken);

        long creditAccountId = existingAccount is not null
            ? existingAccount.Id
            : await _accountRepository.CreateAccountAsync(accountCreateDto, cancellationToken);
        
        await _bonusCalculationHandler.ApplyStartBonusAsync(
            userCreateDto.ExternalUserId,
            userCreateDto.SystemType,
            creditAccountId,
            cancellationToken);

        var existingStats = await _statsRepository.GetStatsByUserIdAsync(
            userCreateDto.ExternalUserId,
            userCreateDto.SystemType,
            cancellationToken);

        if (existingStats == null)
        {
            var userStats = new UserStatsCreateDto
            {
                ExternalUserId = userCreateDto.ExternalUserId
            };
            await _statsRepository.CreateStatsAsync(userStats, cancellationToken);
        }

        var createdUser = await _userRepository.GetUserAsync(userCreateDto.ExternalUserId, userCreateDto.SystemType, cancellationToken);

        return createdUser!;
    }

    /// <inheritdoc />
    public async Task<UserProfileDto> GetProfileAsync(long externalUserId, AuthSystem systemType, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(externalUserId, (byte)systemType, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException($"User with ExternalUserId {externalUserId} not found");
        }

        var account = await _accountRepository.GetAccountByExternalUserIdAsync(user.ExternalUserId, (byte)systemType, cancellationToken);
        var userStat = await _statsRepository.GetStatsByUserIdAsync(user.ExternalUserId, (byte)systemType, cancellationToken);
        var bonusStat = await _bonusStatsRepository.GetStatsByUserIdAsync(user.ExternalUserId, (byte)systemType, cancellationToken);

        var rank = await _rankConfigRepository.GetHighestRankAsync(userStat?.RankPoints ?? 0, cancellationToken);

        var totalDuels = (userStat?.TotalWins ?? 0) + (userStat?.TotalLosses ?? 0);
        var wins = userStat?.TotalWins ?? 0;
        var winrate = totalDuels > 0 ? Math.Round((double)wins / totalDuels * 100, 1) : 0;

        var totalDailyBonus = bonusStat?.TotalDailyBonusesClaimed + bonusStat?.TotalStreakBonusesClaimed;

        var now = _timeProvider.GetUtcNow();
        var canClaimDailyBonus = await _bonusValidator.CanApplyDailyBonus(bonusStat, cancellationToken);
        var timeUntilBonus = bonusStat != null && bonusStat.NextDailyAvailableAt > now 
            ? bonusStat.NextDailyAvailableAt - now
            : null;

        return new UserProfileDto
        {
            ExternalUserId = user.ExternalUserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Balance = account?.Balance ?? 0,
            RankName = rank?.Name ?? "Новичок",
            RankEmoji = rank?.Emoji ?? "🌱",
            RankPoints = userStat?.RankPoints ?? 0,
            TotalDuels = totalDuels,
            Wins = wins,
            Losses = userStat?.TotalLosses ?? 0,
            Winrate = winrate,
            ReferralCount = userStat?.ReferralCount ?? 0,
            TotalReferralBonus = bonusStat?.TotalReferralBonusesClaimed ?? 0,
            CanClaimDailyBonus = canClaimDailyBonus,
            TimeUntilBonus = timeUntilBonus,
            StreakDays = bonusStat?.CurrentDailyStreak ?? 0,
            CreatedAt = user.CreatedAt,
            NextDailyAvailableAt =bonusStat?.NextDailyAvailableAt,
            TotalDailyBonus = totalDailyBonus ?? 0,
        };
    }

    /// <inheritdoc />
    public async Task<bool> ClaimDailyBonusAsync(long externalUserId, AuthSystem systemType, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserAsync(externalUserId, (byte)systemType, cancellationToken);
        if (user == null)
        {
            return false;
        }

        var bonusStat = await _bonusStatsRepository.GetStatsByUserIdAsync(user.ExternalUserId, (byte)systemType, cancellationToken);
        if (bonusStat == null)
        {
            return false;
        }

        var canApplyDailyBonus = await _bonusValidator.CanApplyDailyBonus(bonusStat, cancellationToken);

        if (!canApplyDailyBonus)
        {
            return false;
        }

        var account = await _accountRepository.GetAccountByExternalUserIdAsync(user.ExternalUserId, (byte)systemType, cancellationToken);
        if (account == null)
        {
            return false;
        }

        var dailyBonusAmount = 100.00m;

        var transaction = new TransactionCreateDto
        {
            DebitAccountId = 1,
            CreditAccountId = account.Id,
            Amount = dailyBonusAmount,
            Description = "Daily bonus",
            BonusType = BonusType.Daily,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken);

        var updateBonusDto = new UserBonusStatsUpdateDto
        {
            ExternalUserId = user.ExternalUserId,
            IsStartBonusClaimed = bonusStat.IsStartBonusClaimed,
            CurrentDailyStreak = bonusStat.CurrentDailyStreak + 1,
            TotalDailyBonusesClaimed = bonusStat.TotalDailyBonusesClaimed + dailyBonusAmount,
            LastDailyClaimedAt = _timeProvider.GetUtcNow(),
            NextDailyAvailableAt = _timeProvider.GetUtcNow().AddDays(1)
        };

        await _bonusStatsRepository.UpdateStatsAsync(updateBonusDto, cancellationToken);

        var canApplyStreakBonus = await _bonusValidator.CanApplyStreakBonus(bonusStat, cancellationToken);

        if (canApplyStreakBonus)
        {
            var streakBonusAmount = 1000.00m;

            transaction = new TransactionCreateDto
            {
                DebitAccountId = 1,
                CreditAccountId = account.Id,
                Amount = streakBonusAmount,
                Description = "Streak bonus",
                BonusType = BonusType.Streak,
                CreatedAt = _timeProvider.GetUtcNow()
            };

            await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken);

            updateBonusDto = new UserBonusStatsUpdateDto
            {
                ExternalUserId = user.ExternalUserId,
                IsStartBonusClaimed = bonusStat.IsStartBonusClaimed,
                CurrentDailyStreak = 0,
                TotalStreakBonusesClaimed = bonusStat.TotalStreakBonusesClaimed + streakBonusAmount,
                LastStreakClaimedAt = _timeProvider.GetUtcNow(),
            };

            await _bonusStatsRepository.UpdateStatsAsync(updateBonusDto, cancellationToken);
        }

        return true;
    }
}
