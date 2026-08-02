namespace Mint.Common.Contracts.UserInteractive.Duels;

/// <summary>
/// Represents the status of a duel.
/// </summary>
[Flags]
public enum DuelStatus
{
    /// <summary>
    /// No status.
    /// </summary>
    None = 0,

    /// <summary>
    /// The duel is planned.
    /// </summary>
    Planned = 1,
    
    /// <summary>
    /// The duel is active for votes
    /// </summary>
    Active = 2,

    /// <summary>
    /// The duel is closed for votes.
    /// </summary>
    Closed = 4,
}
