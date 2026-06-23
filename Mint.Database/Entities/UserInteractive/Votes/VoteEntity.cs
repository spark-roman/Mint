using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Duels;

namespace Mint.Database.Entities.UserInteractive.Votes;

/// <summary>
/// Vote entity representing a user's vote in a duel
/// </summary>
[Table("votes")]
public class VoteEntity
{
    /// <summary>
    /// Duel ID
    /// </summary>
    [Required]
    [Column("duel_id")]
    public long DuelId { get; set; }

    /// <summary>
    /// Duel entity
    /// </summary>
    public DuelEntity Duel { get; set; } = null!;

    /// <summary>
    /// Account ID
    /// </summary>
    [Required]
    [Column("account_id")]
    public long AccountId { get; set; }

    /// <summary>
    /// Account entity
    /// </summary>
    [ForeignKey(nameof(AccountId))]
    public AccountEntity Account { get; set; } = null!;

    /// <summary>
    /// Option chosen ('A' or 'B')
    /// </summary>
    [Required]
    [StringLength(1)]
    [Column("option_chosen")]
    public required string OptionChosen { get; set; }

    /// <summary>
    /// Bet amount in coins
    /// </summary>
    [Column("bet_amount")]
    public decimal BetAmount { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
