using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Bonuses.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;

namespace Mint.App.Services.UserInteractive.Profiles.Handlers;

/// <inheritdoc/>
public class UserProfilesHandler(
    IRankConfigRepository rankConfigRepository,
    UserStatsRepository statsRepository,
    IAccountRepository accountRepository,
    IUserBonusStatsRepository bonusStatsRepository) : IUserProfilesHandler
{
    private readonly IRankConfigRepository _rankConfigRepository = rankConfigRepository ?? throw new ArgumentNullException(nameof(rankConfigRepository));

    private readonly UserStatsRepository _statsRepository = statsRepository ?? throw new ArgumentNullException(nameof(statsRepository));

    private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));

    private readonly IUserBonusStatsRepository _bonusStatsRepository = bonusStatsRepository ?? throw new ArgumentNullException(nameof(bonusStatsRepository));

    /// <inheritdoc/>
    public async Task<UserProfileDto?> GetUserProfileAsync(ExternalUserDto userDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userDto);

        var userName = userDto.FirstName ?? userDto.Username ?? string.Empty;
        var ranks = await _rankConfigRepository.GetRankConfigsAsync(cancellationToken);
        var userStats = await _statsRepository.GetStatsByUserIdAsync(userDto.Id, cancellationToken);
        var balance = await _accountRepository.GetUserBalanceAsync(userDto.Id, cancellationToken);
        var bonusStats = await _bonusStatsRepository.GetStatsByUserIdAsync(userDto.Id, cancellationToken);

        userStats ??= new UserStatsDto();

        var profile = new UserProfileDto
        {
            Username = userName,
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
}
