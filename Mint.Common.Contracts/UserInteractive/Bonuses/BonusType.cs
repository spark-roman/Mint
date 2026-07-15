namespace Mint.Common.Contracts.UserInteractive.Bonuses;

/// <summary>
/// Type of bonus.
/// </summary>
public enum BonusType
{
    /// <summary>
    /// No bonus.
    /// </summary>
    None = 0,

    /// <summary>
    /// Start bonus.
    /// </summary>
    Start,

    /// <summary>
    /// Daily bonus.
    /// </summary>
    Daily,

    /// <summary>
    /// Streak bonus.
    /// </summary>
    Streak,

    /// <summary>
    /// Referral bonus.
    /// </summary>
    Referral,

    /// <summary>
    /// Rating bonus.
    /// </summary>
    Rating,

    /// <summary>
    /// Bet.
    /// </summary>
    Bet,

    /// <summary>
    /// Admin bonus.
    /// </summary>
    Admin
}
