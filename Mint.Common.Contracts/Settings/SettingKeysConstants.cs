namespace Mint.Common.Contracts.Settings;

/// <summary>
/// Contains constant keys for system settings stored in the database.
/// </summary>
public static class SettingKeysConstants
{
    /// <summary>
    /// Start bonus amount for new users.
    /// </summary>
    public const string StartBonus = "StartBonus";

    /// <summary>
    /// Daily bonus amount for users who claim it every day.
    /// </summary>
    public const string DailyBonus = "DailyBonus";

    /// <summary>
    /// Bonus amount for users who claim daily bonus 7 days in a row.
    /// </summary>
    public const string StreakBonus = "StreakBonus";

    /// <summary>
    /// Commission percentage taken from the prize pool (e.g., 0.05 = 5%).
    /// </summary>
    public const string HouseCommission = "HouseCommission";

    /// <summary>
    /// Number of hours a duel stays active before expiration.
    /// </summary>
    public const string DuelExpirationHours = "DuelExpirationHours";

    /// <summary>
    /// Number of users displayed in the leaderboard.
    /// </summary>
    public const string LeaderboardSize = "LeaderboardSize";

    /// <summary>
    /// Maximum percentage of user balance allowed for a single bet.
    /// </summary>
    public const string MaxBetPercent = "MaxBetPercent";

    /// <summary>
    /// Bonus amount awarded when a referred friend completes the required actions.
    /// </summary>
    public const string ReferralBonus = "ReferralBonus";

    /// <summary>
    /// Minimum allowed bet amount.
    /// </summary>
    public const string MinBetAmount = "MinBetAmount";

    /// <summary>
    /// Maximum allowed bet amount.
    /// </summary>
    public const string MaxBetAmount = "MaxBetAmount";

    /// <summary>
    /// Maximum number of duels that can be published at once.
    /// </summary>
    public const string MaxDuelsPerPublish = "MaxDuelsPerPublish";

    /// <summary>
    /// Number of news items to process per generation cycle.
    /// </summary>
    public const string NewsBatchSize = "NewsBatchSize";

    /// <summary>
    /// Maximum number of RSS sources to fetch in one cycle.
    /// </summary>
    public const string RssFetchLimit = "RssFetchLimit";
}