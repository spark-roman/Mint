namespace Mint.Common.Contracts.UserInteractive.Payouts;

/// <summary>
/// Represents the status of a payout.
/// </summary>
public enum PayoutStatus
{
    /// <summary>Payout is pending processing.</summary>
    Pending = 0,

    /// <summary>Payout has been completed successfully.</summary>
    Completed = 1,

    /// <summary>Payout processing failed.</summary>
    Failed = 2,

    /// <summary>Payout was cancelled.</summary>
    Cancelled = 3,

    /// <summary>
    /// Payout is owned by an executor that is currently turning the plan into a transaction.
    /// A payout enters this state only through an atomic claim, so no second executor can pick it up.
    /// </summary>
    Processing = 4
}
