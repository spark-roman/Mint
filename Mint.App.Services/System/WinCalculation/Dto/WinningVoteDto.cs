namespace Mint.App.Services.System.WinCalculation.Dto;

/// <summary>
/// Represents a winning vote with calculated payout.
/// </summary>
public sealed record WinningVoteDto
{
    /// <summary>Account identifier.</summary>
    public required long AccountId { get; init; }

    /// <summary>Amount bet.</summary>
    public required decimal BetAmount { get; init; }

    /// <summary>Payout amount (bet + winnings).</summary>
    public required decimal Payout { get; init; }

    /// <summary>Net profit.</summary>
    public decimal Profit => Payout - BetAmount;
}
