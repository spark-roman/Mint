namespace Mint.Common.Contracts.Bot.Commands;

/// <summary>
/// Contains constants for button actions used in the bot.
/// </summary>
public static class ActionConstants
{
    /// <summary>Action for navigating to the main menu.</summary>
    public const string MainMenu = "main_menu";

    /// <summary>Action for navigating to the user profile.</summary>
    public const string Profile = "profile";

    /// <summary>Action for navigating to the duels list.</summary>
    public const string Duels = "duels";

    /// <summary>Action for navigating to the referral program.</summary>
    public const string Referral = "referral";

    /// <summary>Action for claiming the daily bonus.</summary>
    public const string ClaimBonus = "claim_bonus";

    /// <summary>Action for unavailable bonus button (shows notification).</summary>
    public const string BonusUnavailable = "bonus_unavailable"; 

    /// <summary>Action for displaying the leaderboard.</summary>
    public const string Leaderboard = "leaderboard";

    /// <summary>Action for navigating back to the user profile.</summary>
    public const string BackToProfile = "back_to_profile";

    /// <summary>Prefix for category selection actions.</summary>
    public const string CategoryPrefix = "category_";

    /// <summary>Prefix for vote selection actions.</summary>
    public const string VotePrefix = "v_";

    /// <summary>Prefix for bet placement actions.</summary>
    public const string BetPrefix = "bet_";

    /// <summary>Prefix for share actions.</summary>
    public const string SharePrefix = "share_";

    /// <summary>Prefix for cancel actions.</summary>
    public const string CancelPrefix = "cancel_";
}
