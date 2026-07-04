using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.App.Services.UserInteractive.Profiles.Dto;

/// <summary>
/// 
/// </summary>
public record UserProfileDto
{
    /// <summary>
    /// Username of the user.
    /// </summary>
    public string? UserName { get; init; }

    /// <summary>
    /// Rank of the user.
    /// </summary>
    public string Rank { get; init; } = string.Empty;

    /// <summary>
    /// Balance of the user.
    /// </summary>
    public decimal Balance { get; init; }

    /// <summary>
    /// Total wins of the user.
    /// </summary>
    public long TotalWins { get; init; }

    /// <summary>
    /// Total losses of the user.
    /// </summary>
    public long TotalLosses { get; init; }

    /// <summary>
    /// Accuracy of the user.
    /// </summary>
    public double Accuracy => (double)TotalWins / (TotalWins + TotalLosses);

    /// <summary>
    /// Number of referrals made by the user.
    /// </summary>
    public long ReferralCount { get; init; }

    /// <summary>
    /// Earnings from referrals.
    /// </summary>
    public decimal ReferralEarnings { get; init; }

    /// <summary>
    /// Date when the user can make a new bonus.
    /// </summary>
    public DateTimeOffset? NextDailyAvailableAt { get; set; }

        /// <summary>Telegram user identifier.</summary>
    public long ExternalUserId { get; set; }

    /// <summary>First name.</summary>
    public string? FirstName { get; set; }

    /// <summary>Last name.</summary>
    public string? LastName { get; set; }

    /// <summary>Rank name.</summary>
    public string RankName { get; set; } = "Новичок";

    /// <summary>Rank emoji.</summary>
    public string RankEmoji { get; set; } = "🌱";

    /// <summary>Rank points.</summary>
    public int RankPoints { get; set; }

    /// <summary>Total duels participated.</summary>
    public int TotalDuels { get; set; }

    /// <summary>Wins count.</summary>
    public int Wins { get; set; }

    /// <summary>Losses count.</summary>
    public int Losses { get; set; }

    /// <summary>Winrate percentage.</summary>
    public double Winrate { get; set; }

    /// <summary>Can claim bonus now.</summary>
    public bool CanClaimDailyBonus { get; set; }

    /// <summary>Time until next bonus.</summary>
    public TimeSpan? TimeUntilBonus { get; set; }

    /// <summary>Streak days.</summary>
    public int StreakDays { get; set; }

    /// <summary>Created at.</summary>
    public DateTimeOffset CreatedAt { get; set; }
}
