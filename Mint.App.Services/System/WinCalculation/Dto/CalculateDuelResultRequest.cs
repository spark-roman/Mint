namespace Mint.App.Services.System.WinCalculation.Dto;

/// <summary>
/// Request for calculating duel results.
/// </summary>
public sealed record CalculateDuelResultRequest
{
    /// <summary>
    /// Duel identifier.
    /// </summary>
    public required long DuelId { get; init; }

    /// <summary>
    /// Winning option identifier.
    /// </summary>
    public required long WinningOptionId { get; init; }
}
