namespace Mint.Database.Entities.UserInteractive.Bonuses.Dto;

/// <summary>
/// DTO for user bonus stats
/// </summary>
public record UserBonusStatsDto
{
    /// <summary>
    /// Bonus stats ID
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Internal user id
    /// </summary>
    public required long InternalUserId { get; init; }

    /// <summary>
    /// Whether the start bonus has been claimed
    /// </summary>
    public bool IsStartBonusClaimed { get; init; }

    /// <summary>
    /// Current day of continuous daily bonus streak
    /// </summary>
    public int CurrentDailyStreak { get; init; }

    /// <summary>
    /// Time of the last daily bonus claim
    /// </summary>
    public DateTimeOffset? LastDailyClaimedAt { get; init; }

    /// <summary>
    /// Time when the next daily bonus will be available
    /// </summary>
    public DateTimeOffset? NextDailyAvailableAt { get; init; }

    /// <summary>
    /// Total number of start bonuses claimed
    /// </summary>
    public decimal TotalStartBonusesClaimed { get; init; } = 0;

    /// <summary>
    /// Number of referral bonuses claimed
    /// </summary>
    public decimal TotalReferralBonusesClaimed { get; init; } = 0;

    /// <summary>
    /// Total number of daily bonuses claimed
    /// </summary>
    public decimal TotalDailyBonusesClaimed { get; init; } = 0;

    /// <summary>
    /// Number of rating bonuses claimed
    /// </summary>
    public decimal TotalRankBonusClaimed { get; init; } = 0;

    /// <summary>
    /// Time of the last rating bonus claim for duel win
    /// </summary>
    public DateTimeOffset? LastRankBonusClaimedAt { get; init; }

    /// <summary>
    /// Total number of streak bonuses claimed
    /// </summary>
    public decimal TotalStreakBonusesClaimed { get; init; } = 0;

    /// <summary>
    /// Time of the last streak bonus claim
    /// </summary>
    public DateTimeOffset? LastStreakClaimedAt { get; init; }
}
