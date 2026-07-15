using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Mint.App.Services.UserInteractive.Duels.Dto;
using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.Bot.Handlers.Messages;

/// <inheritdoc cref="IMessageFormatter"/>
public sealed class MessageFormatter(TimeProvider timeProvider) : IMessageFormatter
{
    private readonly TimeProvider _timeProvider = timeProvider
        ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc />
    public async Task<string> FormatProfileAsync(string messageTemplate, UserProfileDto userProfileDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userProfileDto);

        var replacements = new Dictionary<string, string>
        {
            ["{{user_id}}"] = userProfileDto.ExternalUserId.ToString(CultureInfo.InvariantCulture),
            ["{{username}}"] = userProfileDto.UserName ?? "Не указан",
            ["{{first_name}}"] = userProfileDto.FirstName ?? "",
            ["{{last_name}}"] = userProfileDto.LastName ?? "",
            ["{{balance}}"] = userProfileDto.Balance.ToString("N0", CultureInfo.InvariantCulture) ?? "0",
            ["{{total_duels}}"] = userProfileDto.TotalDuels.ToString(CultureInfo.InvariantCulture) ?? "0",
            ["{{wins}}"] = userProfileDto.TotalWins.ToString(CultureInfo.InvariantCulture) ?? "0",
            ["{{losses}}"] = userProfileDto.TotalLosses.ToString(CultureInfo.InvariantCulture) ?? "0",
            ["{{winrate}}"] = userProfileDto.Winrate.ToString(CultureInfo.InvariantCulture),
            ["{{rank_name}}"] = userProfileDto.RankName ?? "Новичок",
            ["{{rank_emoji}}"] = userProfileDto.RankEmoji ?? "🌱",
            ["{{rank_points}}"] = userProfileDto.RankPoints.ToString(CultureInfo.InvariantCulture) ?? "",
            ["{{referral_count}}"] = userProfileDto.ReferralCount.ToString(CultureInfo.InvariantCulture) ?? "0",
            ["{{total_referral_bonus}}"] = userProfileDto.TotalReferralBonus.ToString("N0", CultureInfo.InvariantCulture) ?? "0",
            ["{{streak_days}}"] = userProfileDto.StreakDays.ToString("N0", CultureInfo.InvariantCulture) ?? "0",
            ["{{total_daily_bonus}}"] = userProfileDto.TotalDailyBonus.ToString("N0", CultureInfo.InvariantCulture) ?? "0",
            ["{{member_since}}"] = userProfileDto.CreatedAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "",
        };

        if (_timeProvider.GetUtcNow() >= userProfileDto.NextDailyAvailableAt)
        {
            replacements["{{bonus_status}}"] = "Доступен 🎁";
        }
        else
        {
            var timeLeft = userProfileDto.NextDailyAvailableAt - _timeProvider.GetUtcNow();
            replacements["{{bonus_status}}"] = $"Через {timeLeft:hh\\:mm\\:ss}";
        }

        var resultMessage = replacements.Aggregate(messageTemplate, (current, kvp) => current.Replace(kvp.Key, kvp.Value, StringComparison.InvariantCultureIgnoreCase));

        return await Task.FromResult(resultMessage);
    }

    /// <inheritdoc />
    public async Task<string> FormatLeaderboardAsync(string messageTemplate, LeaderboardResultDto leaderboardResult, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(leaderboardResult);

        var replacements = new Dictionary<string, string>
        {
            ["{{leaderboard_entries}}"] = BuildLeaderboardEntries(leaderboardResult.Entries),
            ["{{user_rank_info}}"] = BuildUserRankInfo(leaderboardResult.UserRank, leaderboardResult.UserEntry),
            ["{{total_users}}"] = leaderboardResult.TotalUsers.ToString(CultureInfo.InvariantCulture)
        };

        return await Task.FromResult(replacements.Aggregate(
            messageTemplate,
            (current, kvp) => current.Replace(kvp.Key, kvp.Value, StringComparison.InvariantCultureIgnoreCase)));
    }

    private static string BuildLeaderboardEntries(IReadOnlyCollection<LeaderboardEntryDto> entries)
    {
        if (entries == null || entries.Count == 0)
        {
            return "Пока нет участников. Станьте первым! 🚀";
        }

        var sb = new StringBuilder();

        foreach (var entry in entries)
        {
            var medal = entry.Rank switch
            {
                1 => "🥇",
                2 => "🥈",
                3 => "🥉",
                _ => "🎖"
            };

            sb.AppendLine(CultureInfo.CurrentCulture, $"{medal} **{entry.Rank}.** {entry.DisplayName} — {entry.RankName} • {entry.RankPoints:N0} RP");
        }

        return sb.ToString();
    }

    private static string BuildUserRankInfo(int? userRank, LeaderboardEntryDto? userEntry)
    {
        if (!userRank.HasValue || userEntry == null)
        {
            return "👤 **Вы ещё не участвовали в дуэлях**";
        }

        return $"👤 **Ваше место в рейтинге:** #{userRank.Value} ({userEntry.RankPoints:N0} RP)";
    }

        /// <inheritdoc />
    public Task<string> FormatDuelAsync(string messageTemplate, DuelCardDto duelCard, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(duelCard);

        var replacements = new Dictionary<string, string>
        {
            ["{{duel_id}}"] = duelCard.DuelId.ToString(CultureInfo.InvariantCulture),
            ["{{category_name}}"] = duelCard.CategoryName,
            ["{{question}}"] = duelCard.Question,
            ["{{description}}"] = duelCard.Description,
            ["{{time_left}}"] = duelCard.TimeLeft
        };

        var formattedMessage = replacements.Aggregate(messageTemplate, (current, kvp) => current.Replace(kvp.Key, kvp.Value, StringComparison.InvariantCultureIgnoreCase));

        return Task.FromResult(formattedMessage);
    }

    /// <inheritdoc />
    public Task<string> FormatBetAsync(string messageTemplate, BetDataDto betData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(betData);

        var replacements = new Dictionary<string, string>
        {
            ["{{selected_option}}"] = betData.SelectedOption,
            ["{{balance}}"] = betData.Balance.ToString("N0", CultureInfo.InvariantCulture)
        };

        var formattedMessage = replacements.Aggregate(messageTemplate, (current, kvp) => current.Replace(kvp.Key, kvp.Value, StringComparison.InvariantCultureIgnoreCase));

        return Task.FromResult(formattedMessage);
    }

    /// <inheritdoc />
    public Task<string> FormatSuccessAsync(string messageTemplate, SuccessDataDto successData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(successData);

        var replacements = new Dictionary<string, string>
        {
            ["{{selected_option}}"] = successData.SelectedOption,
            ["{{bet_amount}}"] = successData.BetAmount.ToString("N0", CultureInfo.InvariantCulture),
            ["{{time_left}}"] = successData.TimeLeft
        };

        var formattedMessage = replacements.Aggregate(messageTemplate, (current, kvp) => current.Replace(kvp.Key, kvp.Value, StringComparison.InvariantCultureIgnoreCase));

        return Task.FromResult(formattedMessage);
    }

    /// <inheritdoc />
    public Task<string> FormatCategoriesAsync(string messageTemplate, Collection<CategoryDto> categories, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(messageTemplate);

        var categoriesList = categories
            .Select(c => $"• {c.Name}")
            .Aggregate((a, b) => $"{a}\n{b}");

        return Task.FromResult(messageTemplate.Replace("{{categories_list}}", categoriesList, StringComparison.InvariantCultureIgnoreCase));
    }
}
