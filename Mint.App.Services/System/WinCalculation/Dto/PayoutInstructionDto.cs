namespace Mint.App.Services.System.WinCalculation.Dto;

/// <summary>
/// Represents a payout instruction.
/// </summary>
public sealed record PayoutInstructionDto
{
    /// <summary>
    /// Account id of the credit account.
    /// </summary>
    public required long CreditAccountId { get; init; }

    /// <summary>
    /// Amount to be paid.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// External user id.
    /// </summary>
    public required long ExternalUserId { get; init; }
}
