using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.Users;

namespace Mint.Database.Entities.UserInteractive.Stats;

/// <summary>
/// User stats entity
/// </summary>
[Table("user_stats")]
public class UserStatsEntity
{
    /// <summary>
    /// Stats ID
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
    /// Accumulated rank points
    /// </summary>
    [Column("rank_points")]
    public decimal RankPoints { get; set; } = 0;

    /// <summary>
    /// Number of won duels
    /// </summary>
    [Column("total_wins")]
    public int TotalWins { get; set; } = 0;

    /// <summary>
    /// Number of lost duels
    /// </summary>
    [Column("total_losses")]
    public int TotalLosses { get; set; } = 0;

    /// <summary>
    /// Last update timestamp
    /// Number of successfully referred friends who completed the conditions
    /// </summary>
    [Column("referral_count")]
    public int ReferralCount { get; set; } = 0;

    /// <summary>
    /// Last update timestamp
    /// </summary>
    [Column("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
}
