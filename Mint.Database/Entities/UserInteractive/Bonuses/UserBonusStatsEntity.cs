using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.Users;

namespace Mint.Database.Entities.UserInteractive.Bonuses;

/// <summary>
/// User bonus stats entity
/// </summary>
[Table("user_bonus_stats")]
public class UserBonusStatsEntity
{
    /// <summary>
    /// Bonus stats ID
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public long Id { get; set; }

    /// <summary>
    /// User ID (foreign key)
    /// </summary>
    [Required]
    [Column("user_id")]
    public long UserId { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public virtual UserEntity User { get; set; } = null!;

    /// <summary>
    /// Whether the start bonus has been claimed
    /// </summary>
    [Column("is_start_bonus_claimed")]
    public bool IsStartBonusClaimed { get; set; } = false;

    /// <summary>
    /// Total number of start bonuses claimed
    /// </summary>
    public decimal TotalStartBonusesClaimed { get; set; } = 0;

    /// <summary>
    /// Current day of continuous daily bonus streak
    /// </summary>
    [Column("current_daily_streak")]
    public int CurrentDailyStreak { get; set; } = 0;

    /// <summary>
    /// Total number of streak bonuses claimed
    /// </summary>
    [Column("total_streak_bonuses_claimed")]
    public decimal TotalStreakBonusesClaimed { get; set; } = 0;

    /// <summary>
    /// Time of the last streak bonus claim
    /// </summary>
    [Column("last_streak_claimed_at")]
    public DateTimeOffset? LastStreakClaimedAt { get; set; }

    /// <summary>
    /// Total number of daily bonuses claimed
    /// </summary>
    [Column("total_daily_bonuses_claimed")]
    public decimal TotalDailyBonusesClaimed { get; set; } = 0;
    
    /// <summary>
    /// Time of the last daily bonus claim
    /// </summary>
    [Column("last_daily_claimed_at")]
    public DateTimeOffset? LastDailyClaimedAt { get; set; }

    /// <summary>
    /// Time when the next daily bonus will be available
    /// </summary>
    [Column("next_daily_available_at")]
    public DateTimeOffset? NextDailyAvailableAt { get; set; }

    /// <summary>
    /// Number of referral bonuses claimed
    /// </summary>
    [Column("total_referral_bonuses_claimed")]
    public decimal TotalReferralBonusesClaimed { get; set; } = 0;

    /// <summary>
    /// Total number of rank bonuses claimed
    /// </summary>
    [Column("total_rank_bonus_claimed")]
    public decimal TotalRankBonusClaimed { get; set; } = 0;

    /// <summary>
    /// Time of the last rating bonus claim for duel win
    /// </summary>
    [Column("last_rank_bonus_claimed_at")]
    public DateTimeOffset? LastRankBonusClaimedAt { get; set; }
}
