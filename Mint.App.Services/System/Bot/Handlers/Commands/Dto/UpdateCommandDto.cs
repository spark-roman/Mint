using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands.Dto;

/// <summary>
/// Map update tg command to UpdateCommandDto
/// </summary>
public record UpdateCommandDto
{
    /// <summary>
    /// Command text
    /// </summary>
    public string? CommandText { get; init; }

    /// <summary>
    /// Callback data
    /// </summary>
    public string? CallbackData { get; init; }

    /// <summary>
    /// Callback id
    /// </summary>
    public string? CallbackId { get; init; }

    /// <summary>
    /// Chat id
    /// </summary>
    public long ChatId { get; init; }

    /// <summary>
    /// User
    /// </summary>
    public User? User  { get; init; }

    /// <summary>
    /// Message id
    /// </summary>
    public int MessageId { get; set; }
}
