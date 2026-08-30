namespace Mint.App.Services.System.Bot.Dto;

/// <summary>
/// Telegram bot configuration options.
/// </summary>
public sealed class TelegramOptions
{
    /// <summary>
    /// Section name for configuration.
    /// </summary>
    public const string SectionName = "Auth:Tg";

    /// <summary>
    /// Bot username (e.g., "opinion_bot").
    /// </summary>
    public required string BotUsername { get; set; }
}
