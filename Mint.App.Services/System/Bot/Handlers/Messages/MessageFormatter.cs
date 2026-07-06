using System.Globalization;
using Mint.App.Services.UserInteractive.Profiles.Dto;

namespace Mint.App.Services.System.Bot.Handlers.Messages;

/// <inheritdoc cref="IMessageFormatter"/>
public sealed class MessageFormatter(TimeProvider timeProvider) : IMessageFormatter
{
    private readonly TimeProvider _timeProvider = timeProvider
        ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc />
    public async Task<string> FormatAsync(string messageTemplate, UserProfileDto userProfileDto, CancellationToken cancellationToken)
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
}
