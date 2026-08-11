using Mint.Common.Contracts.UserInteractive.Payouts;

namespace Mint.Database.Entities.System.Payouts.Dto;

/// <summary>
/// Data transfer object for updating a payout status.
/// </summary>
public sealed record PayoutUpdateDto
{
    /// <summary>Unique identifier of the payout.</summary>
    public required long Id { get; init; }

    /// <summary>New status of the payout.</summary>
    public required PayoutStatus Status { get; init; }

    /// <summary>Identifier of the associated transaction.</summary>
    public long? TransactionId { get; init; }
}

