namespace Mint.App.Services.UserInteractive.Duels.Dto;

/// <summary>
/// Result of a bet placement operation.
/// </summary>
public sealed record BetResultDto
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Optional message indicating the result of the operation.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Balance after the bet placement operation.
    /// </summary>
    public decimal NewBalance { get; init; }

    /// <summary>
    /// Optional vote id for the placed bet.
    /// </summary>
    public long VoteId { get; init; }
}
