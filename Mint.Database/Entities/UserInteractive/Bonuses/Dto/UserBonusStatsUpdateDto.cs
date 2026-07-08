namespace Mint.Database.Entities.UserInteractive.Bonuses.Dto;

/// <summary>
/// DTO for updating user bonus stats
/// </summary>
public record UserBonusStatsUpdateDto
{
    /// <summary>
    /// External user id
    /// </summary>
    public required long ExternalUserId { get; init; }

    /// <summary>
    /// Internal user id
    /// </summary>
    public long InternalUserId { get; set; }

    /// <summary>
    /// Whether the start bonus has been claimed
    /// </summary>
    public bool IsStartBonusClaimed { get; init; }

    /// <summary>
    /// Time of the last start bonus claim
    /// </summary>
    public DateTimeOffset? StartBonusClaimedAt { get; init; }

    /// <summary>
    /// Total number of start bonuses claimed
    /// </summary>
    public decimal TotalStartBonusesClaimed { get; init; } = 0;

    /// <summary>
    /// Current day of continuous daily bonus streak
    /// </summary>
    public int CurrentDailyStreak { get; init; }

    /// <summary>
    /// Total number of streak bonuses claimed
    /// </summary>
    public decimal TotalStreakBonusesClaimed { get; init; } = 0;

    /// <summary>
    /// Time of the last streak bonus claim
    /// </summary>
    public DateTimeOffset? LastStreakClaimedAt { get; init; }

    /// <summary>
    /// Total number of daily bonuses claimed
    /// </summary>
    public decimal TotalDailyBonusesClaimed { get; init; } = 0;

    /// <summary>
    /// Time of the last daily bonus claim
    /// </summary>
    public DateTimeOffset? LastDailyClaimedAt { get; init; }

    /// <summary>
    /// Time when the next daily bonus will be available
    /// </summary>
    public DateTimeOffset? NextDailyAvailableAt { get; init; }

    /// <summary>
    /// Number of referral bonuses claimed
    /// </summary>
    public decimal TotalReferralBonusesClaimed { get; init; } = 0;

    /// <summary>
    /// Total number of rank bonuses claimed
    /// </summary>
    public decimal TotalRankBonusClaimed { get; init; } = 0;

    /// <summary>
    /// Time of the last rating bonus claim for duel win
    /// </summary>
    public DateTimeOffset? LastRankBonusClaimedAt { get; init; }
}
