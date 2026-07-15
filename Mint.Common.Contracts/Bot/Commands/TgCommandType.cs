namespace Mint.Common.Contracts.Bot.Commands;

/// <summary>
/// Represents all possible command types that can be handled by the Telegram bot.
/// </summary>
public enum TgCommandType
{
    // ========== Базовые команды (1-9) ==========
    
    /// <summary>
    /// Not supported or unknown command.
    /// </summary>
    None = 0,

    /// <summary>
    /// Start command - initializes user and shows main menu.
    /// </summary>
    Start = 1,

    /// <summary>
    /// Help command - shows available commands and instructions.
    /// </summary>
    Help = 2,

    // ========== Навигационные команды (10-19) ==========
    
    /// <summary>
    /// Profile command - shows user profile with statistics and bonuses.
    /// </summary>
    Profile = 10,

    /// <summary>
    /// Duels command - shows category selection for duels.
    /// </summary>
    Duels = 11,

    /// <summary>
    /// Referral command - shows referral program and link.
    /// </summary>
    Referral = 12,

    /// <summary>
    /// Main menu command - navigates back to the main menu.
    /// </summary>
    MainMenu = 13,

    /// <summary>
    /// Leaderboard command - shows the leaderboard.
    /// </summary>
    Leaderboard = 14,

    /// <summary>
    /// Back to profile command - navigates back to user profile.
    /// </summary>
    BackToProfile = 15,

    // ========== Бонусные команды (20-29) ==========
    
    /// <summary>
    /// Claim bonus command - claims daily bonus for the user.
    /// </summary>
    ClaimBonus = 20,

    /// <summary>
    /// Bonus unavailable command - handles click on unavailable bonus button.
    /// </summary>
    BonusUnavailable = 21,

    // ========== Дуэльные команды (30-39) ==========
    
    /// <summary>
    /// Category selection command - handles category selection in duels.
    /// </summary>
    CategorySelection = 30,

    /// <summary>
    /// Duel selection command - handles duel selection.
    /// </summary>
    DuelSelection = 31,

    /// <summary>
    /// Vote command - handles vote/option selection in a duel.
    /// </summary>
    Vote = 32,

    /// <summary>
    /// Bet placement command - handles bet placement in a duel.
    /// </summary>
    BetPlacement = 33,

    /// <summary>
    /// Cancel command - cancels current bet and returns to duel card.
    /// </summary>
    Cancel = 34,

    /// <summary>
    /// Share command - handles share/invite actions.
    /// </summary>
    Share = 35,

    // ========== Вводные команды (40-49) ==========
    
    /// <summary>
    /// Text input command - handles text input from user (e.g., custom bet amount).
    /// </summary>
    TextInput = 40,

    /// <summary>
    /// Number input command - handles numeric input (e.g., bet amount).
    /// </summary>
    NumberInput = 41,

    // ========== Callback команды (50-59) ==========
    
    /// <summary>
    /// Generic callback command - handles generic callback queries.
    /// </summary>
    Callback = 50,

    /// <summary>
    /// Callback navigation command - handles navigation via callbacks.
    /// </summary>
    CallbackNavigation = 51,

    // ========== Административные команды (90-99) ==========
    
    /// <summary>
    /// Admin command - for administrative operations.
    /// </summary>
    Admin = 90,

    /// <summary>
    /// Maintenance command - for system maintenance.
    /// </summary>
    Maintenance = 91
}