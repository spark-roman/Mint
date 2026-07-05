namespace Mint.Common.Contracts.Bot.Commands;

/// <summary>
/// Telgram handled commands
/// </summary>
public enum TgCommandType
{
    /// <summary>
    /// Not supported command
    /// </summary>
    None = 0,

    /// <summary>
    /// Start command
    /// </summary>
    Start = 1,

    /// <summary>
    /// Help command
    /// </summary>
    Help = 2,

    /// <summary>
    /// Profile command
    /// </summary>
    Profile = 10,

    /// <summary>
    /// Duels command
    /// </summary>
    Duels = 11,

    /// <summary>
    /// Referral command
    /// </summary>
    Referral = 12,

    /// <summary>
    /// Main menu command
    /// </summary>
    MainMenu = 13,

    /// <summary>
    /// Claim bonus command
    /// </summary>
    ClaimBonus = 20,

    /// <summary>
    /// Leaderboard command
    /// </summary>
    Leaderboard = 21,

    /// <summary>
    /// Category selection command
    /// </summary>
    CategorySelection = 30,

    /// <summary>
    /// Duel selection command
    /// </summary>
    DuelSelection = 31,

    /// <summary>
    /// Bet placement command
    /// </summary>
    BetPlacement = 32,

    /// <summary>
    /// Text input command
    /// </summary>
    TextInput = 40,

    /// <summary>
    /// Callback command
    /// </summary>
    Callback = 41,

    /// <summary>
    /// Callback navigation command
    /// </summary>
    CallbackNavigation = 42
}
