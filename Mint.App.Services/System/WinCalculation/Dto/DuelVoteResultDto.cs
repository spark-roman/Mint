using Mint.Database.Entities.Ledger.Transactions.Dto;

namespace Mint.App.Services.System.WinCalculation.Dto;

/// <summary>
/// 
/// </summary>
public sealed record DuelVoteResultDto
{
    /// <summary>
    /// Winning payout transaction
    /// </summary>
    public TransactionCreateDto? PayoutInstruction { get; init; }

    /// <summary>
    /// Vote account id
    /// </summary>
    public long VoteAccountId { get; init; }

    /// <summary>
    /// Vote id
    /// </summary>
    public long VoteId { get; set; }
}
