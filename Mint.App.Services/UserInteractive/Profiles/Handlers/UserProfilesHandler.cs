using AdvApplication.Auth.Users;
using Mint.App.Services.UserInteractive.Bonuses.Rules;
using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.App.Services.UserInteractive.Users.Dto;
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
    IRankConfigRepository rankConfigRepository,
    IUserStatsRepository statsRepository,
    IAccountRepository accountRepository,
    IUserBonusStatsRepository bonusStatsRepository,
    IUserRepository userRepository,
    ITransactionRepository transactionRepository,
    TimeProvider timeProvider,
    IBonusValidator bonusValidator) : IUserProfilesHandler
{
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

    /// <inheritdoc/>
    public async Task<UserProfileDto?> GetUserProfileAsync(ExternalUserDto userDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userDto);

        var userName = userDto.FirstName ?? userDto.Username ?? string.Empty;
        var ranks = await _rankConfigRepository.GetRankConfigsAsync(cancellationToken);
        var userStats = await _statsRepository.GetStatsByUserIdAsync(userDto.ExternalUserId, cancellationToken);
        var balance = await _accountRepository.GetUserBalanceAsync(userDto.ExternalUserId, cancellationToken);
        var bonusStats = await _bonusStatsRepository.GetStatsByUserIdAsync(userDto.ExternalUserId, cancellationToken);

        userStats ??= new UserStatsDto();

        var profile = new UserProfileDto
        {
            UserName = userName,
            Rank = ComputeRank(ranks, userStats),
            Balance = balance,
            TotalWins = userStats.TotalWins,
            TotalLosses = userStats.TotalLosses,
            ReferralCount = userStats.ReferralCount,
            ReferralEarnings = userStats.ReferralEarnings,
            NextDailyAvailableAt = bonusStats is null ? default : bonusStats.NextDailyAvailableAt
        };

        return profile;
    }

    private static string ComputeRank(List<RankConfigDto> ranks, UserStatsDto userStats)
    {
        return ranks
            .Where(r => r.MinPoints <= userStats.RankPoints)
            .OrderByDescending(r => r.MinPoints)
            .Select(r => $"{r.Emoji} {r.Name}")
            .First();
    }

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
        
        var bonusStat = await _bonusStatsRepository.GetStatsByUserIdAsync(userCreateDto.ExternalUserId, cancellationToken);

        if (await _bonusValidator.CanApplyStartBonus(bonusStat, cancellationToken))
        {
            var transaction = new TransactionCreateDto
            {
                DebetAccountId = 1,
                CreditAccountId = creditAccountId,
                Amount = 100,
                Description = "Start bonus",
                BounusType = BonusType.Start,
                CreatedAt = _timeProvider.GetUtcNow()
            };

            await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken);

            var userStats = new UserStatsCreateDto
            {
                ExternalUserId = userCreateDto.ExternalUserId
            };

            await _statsRepository.CreateStatsAsync(userStats, cancellationToken);

            var bonusStats = new UserBonusStatsCreateDto
            {
               ExternalUserId = userCreateDto.ExternalUserId,
               IsStartBonusClaimed = true,
               StartBonusClaimedAt = _timeProvider.GetUtcNow()
            };

            await _bonusStatsRepository.CreateStatsAsync(bonusStats, cancellationToken);
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
        var userStat = await _statsRepository.GetStatsByUserIdAsync(user.ExternalUserId, cancellationToken);
        var bonusStat = await _bonusStatsRepository.GetStatsByUserIdAsync(user.ExternalUserId, cancellationToken);

        var rank = await _rankConfigRepository.GetHighestRankAsync(userStat?.RankPoints ?? 0, cancellationToken);

        var totalDuels = (userStat?.TotalWins ?? 0) + (userStat?.TotalLosses ?? 0);
        var wins = userStat?.TotalWins ?? 0;
        var winrate = totalDuels > 0 ? Math.Round((double)wins / totalDuels * 100, 1) : 0;

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
            ReferralEarnings = userStat?.ReferralEarnings ?? 0,
            CanClaimDailyBonus = canClaimDailyBonus,
            TimeUntilBonus = timeUntilBonus,
            StreakDays = bonusStat?.CurrentDailyStreak ?? 0,
            CreatedAt = user.CreatedAt
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

        var bonusStat = await _bonusStatsRepository.GetStatsByUserIdAsync(user.ExternalUserId, cancellationToken);
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

        var transaction = new TransactionCreateDto
        {
            DebetAccountId = 1,
            CreditAccountId = account.Id,
            Amount = 100,
            Description = "Daily bonus",
            BounusType = BonusType.Daily,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken);

        var updateBonusDto = new UserBonusStatsUpdateDto
        {
            ExternalUserId = user.ExternalUserId,
            IsStartBonusClaimed = bonusStat.IsStartBonusClaimed,
            CurrentDailyStreak = bonusStat.CurrentDailyStreak + 1,
            LastDailyClaimedAt = _timeProvider.GetUtcNow()
        };

        await _bonusStatsRepository.UpdateStatsAsync(updateBonusDto, cancellationToken);

        var canApplyStreakBonus = await _bonusValidator.CanApplyStreakBonus(bonusStat, cancellationToken);

        if (canApplyStreakBonus)
        {
            transaction = new TransactionCreateDto
            {
                DebetAccountId = 1,
                CreditAccountId = account.Id,
                Amount = 1000,
                Description = "Streak bonus",
                BounusType = BonusType.Streak,
                CreatedAt = _timeProvider.GetUtcNow()
            };

            await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken);

            updateBonusDto = new UserBonusStatsUpdateDto
            {
                ExternalUserId = user.ExternalUserId,
                IsStartBonusClaimed = bonusStat.IsStartBonusClaimed,
                CurrentDailyStreak = 0,
                LastDailyClaimedAt = _timeProvider.GetUtcNow()
            };

            await _bonusStatsRepository.UpdateStatsAsync(updateBonusDto, cancellationToken);
        }

        return true;
    }

    /// <inheritdoc />
    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int limit, AuthSystem systemType, CancellationToken cancellationToken)
    {
        var stats = await _statsRepository.GetTopStatsByUserIdAsync(limit, cancellationToken);
        var result = new List<LeaderboardEntryDto>();

        for (int i = 0; i < stats.Count; i++)
        {
            var stat = stats[i];
            var user = await _userRepository.GetUserAsync(stat.ExternalUserId, (byte)systemType, cancellationToken);
            var totalDuels = stat.TotalWins + stat.TotalLosses;
            var winrate = totalDuels > 0 ? Math.Round((double)stat.TotalWins / totalDuels * 100, 1) : 0;

            result.Add(new LeaderboardEntryDto
            {
                Rank = i + 1,
                ExternalUserId = user?.ExternalUserId ?? 0,
                DisplayName = user?.FirstName ?? user?.UserName ?? "Аноним",
                RankPoints = stat.RankPoints,
                TotalDuels = totalDuels,
                Winrate = winrate
            });
        }

        return result;
    }
}
