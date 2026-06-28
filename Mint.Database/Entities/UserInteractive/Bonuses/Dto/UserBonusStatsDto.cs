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
    /// Number of referral bonuses claimed
    /// </summary>
    public int TotalReferralBonusesClaimed { get; init; }

    /// <summary>
    /// Time of the last rating bonus claim for duel win
    /// </summary>
    public DateTimeOffset? LastRatingBonusClaimedAt { get; init; }
}
