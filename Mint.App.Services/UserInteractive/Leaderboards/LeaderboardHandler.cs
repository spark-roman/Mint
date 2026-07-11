using AdvApplication.Auth.Users;
using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;

namespace Mint.App.Services.UserInteractive.Leaderboards;

/// <inheritdoc cref="ILeaderboardHandler"/>
public sealed class LeaderboardHandler(
    IUserStatsRepository statsRepository,
    IRankConfigRepository rankRepository,
    IUserRepository userRepository) : ILeaderboardHandler
{
    private readonly IUserStatsRepository _statsRepository = statsRepository ?? throw new ArgumentNullException(nameof(statsRepository));

    private readonly IRankConfigRepository _rankRepository = rankRepository ?? throw new ArgumentNullException(nameof(rankRepository));

    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private const int DefaultTop = 15;

    /// <inheritdoc/>
    public async Task<LeaderboardResultDto> GetLeaderboardAsync(
        int top,
        long externalUserId,
        AuthSystem authSystem,
        CancellationToken cancellationToken)
    {
        if (top <= 0)
        {
            top = DefaultTop;
        }
        
        var topStats = await _statsRepository.GetTopStatsByUserIdAsync(top, cancellationToken);

        if (topStats.Count == 0)
        {
            return new LeaderboardResultDto
            {
                Entries = new List<LeaderboardEntryDto>().AsReadOnly(),
                TotalUsers = 0,
                UserRank = null,
                UserEntry = null
            };
        }

        var rankCache = new Dictionary<decimal, string>();

        foreach (var stat in topStats)
        {
            if (!rankCache.ContainsKey(stat.RankPoints))
            {
                var rank = await _rankRepository.GetHighestRankAsync(stat.RankPoints, cancellationToken);
                rankCache[stat.RankPoints] = rank != null ? $"{rank.Emoji} {rank.Name}" : "🌱 Новичок";
            }
        }

        var entries = topStats
            .Select((stat, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                ExternalUserId = stat.ExternalUserId,
                DisplayName = stat.UserName,
                RankName = rankCache.GetValueOrDefault(stat.RankPoints, "🌱 Новичок"),
                RankPoints = stat.RankPoints
            })
            .ToList();

        LeaderboardEntryDto? userEntry = null;
        int? userRank = null;

        var userStat = topStats.FirstOrDefault(s => s.ExternalUserId == externalUserId);

        if (userStat != null)
        {
            userRank = topStats.Count(s => s.RankPoints > userStat.RankPoints) + 1;
            userEntry = entries.First(e => e.ExternalUserId == externalUserId);
        }
        else
        {
            var userStats = await _statsRepository.GetStatsByUserIdAsync(externalUserId, (byte)authSystem, cancellationToken);
            if (userStats != null)
            {
                userRank = await _statsRepository.GetUserRankByPointsAsync(userStats.RankPoints, cancellationToken);
                var rank = await _rankRepository.GetHighestRankAsync(userStats.RankPoints, cancellationToken);
                var rankName = rank != null ? $"{rank.Emoji} {rank.Name}" : "🌱 Новичок";

                userEntry = new LeaderboardEntryDto
                {
                    Rank = userRank.Value,
                    ExternalUserId = userStats.ExternalUserId,
                    DisplayName = userStats.UserName ?? "Аноним",
                    RankName = rankName,
                    RankPoints = userStats.RankPoints
                };
            }
        }

        var totalUsers = await _userRepository.GetTotalUsersCountAsync(cancellationToken);

        return new LeaderboardResultDto
        {
            Entries = entries.AsReadOnly(),
            TotalUsers = totalUsers,
            UserRank = userRank,
            UserEntry = userEntry
        };
    }
}