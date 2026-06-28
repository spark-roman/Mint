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
    /// Account ID (part of composite key)
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
    /// Duel ID (part of composite key)
    /// </summary>
    [Required]
    [Column("duel_id")]
    public long DuelId { get; set; }

    /// <summary>
    /// Parent duel
    /// </summary>
    public DuelEntity Duel { get; set; } = null!;

    /// <summary>
    /// Chosen option ID (FK to duel_options)
    /// </summary>
    [Required]
    [Column("chosen_option_id")]
    public long ChosenOptionId { get; set; }

    /// <summary>
    /// Chosen option
    /// </summary>
    [ForeignKey(nameof(ChosenOptionId))]
    public DuelOptionEntity ChosenOption { get; set; } = null!;

    /// <summary>
    /// Bet amount in coins
    /// </summary>
    [Required]
    [Column("bet_amount")]
    public decimal BetAmount { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
