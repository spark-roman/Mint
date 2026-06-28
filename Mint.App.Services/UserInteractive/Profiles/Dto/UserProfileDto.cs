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
    public string? Username { get; init; }

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
}
