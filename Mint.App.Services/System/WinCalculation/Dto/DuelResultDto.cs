using Mint.Database.Entities.Ledger.Transactions.Dto;

namespace Mint.App.Services.System.WinCalculation.Dto;

/// <summary>
/// Represents the result of a duel calculation.
/// </summary>
public sealed record DuelResultDto
{
    /// <summary>Identifier of the duel.</summary>
    public required long DuelId { get; init; }

    /// <summary>Type of the duel (1 = OpinionMatch, 2 = FactPrediction).</summary>
    public required int DuelType { get; init; }

    /// <summary>Winning option identifier.</summary>
    public required long WinningOptionId { get; init; }

    /// <summary>Total pot of the duel.</summary>
    public required decimal TotalPot { get; init; }

    /// <summary>House cut (commission).</summary>
    public required decimal HouseCut { get; init; }

    /// <summary>Prize pool (total pot - house cut).</summary>
    public decimal PrizePool => TotalPot - HouseCut;

    /// <summary>List of payout transactions.</summary>
    public required ICollection<TransactionCreateDto> PayoutInstructions { get; init; }

    /// <summary>House cut (commission) taken by the bot.</summary>
    public decimal HouseCutPercent { get; init; } = 0.05m; // 5% комиссии

    /// <summary>Whether the duel is finalized.</summary>
    public bool IsFinalized { get; init; }
}
