namespace Mint.App.Services.System.Bot.Dto;

/// <summary>
/// Telegram bot configuration options.
/// </summary>
public sealed class TelegramOptions
{
    /// <summary>
    /// Section name for configuration.
    /// </summary>
    public const string SectionName = "Auth:Tg:UserName";

    /// <summary>
    /// Bot username (e.g., "opinion_bot").
    /// </summary>
    public string BotUsername { get; set; } = string.Empty;
}
