namespace Mint.App.Services.UserInteractive.Referral.Dto;

/// <summary>
/// Data transfer object for referral information used in message formatting.
/// </summary>
public sealed record ReferralDataDto
{
    /// <summary>
    /// Unique referral code for the user (e.g., "Mj37X1K9").
    /// </summary>
    public required string ReferralCode { get; init; }

    /// <summary>
    /// Number of users who have been referred by this user.
    /// </summary>
    public required int ReferralCount { get; init; }

    /// <summary>
    /// Bonus amount awarded for each successful referral.
    /// </summary>
    public required decimal ReferralAmount { get; init; }

    /// <summary>
    /// Username of the bot (e.g., "opinion_bot").
    /// </summary>
    public required string BotUserName { get; init; }
}