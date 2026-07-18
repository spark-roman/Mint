using System.Collections.ObjectModel;
using Mint.Database.Entities.Bot.Commands.Dto;

namespace Mint.App.Services.System.Bot.Dto;

/// <summary>
/// Result of a command or button handler.
/// </summary>
public record CommandResult
{
    /// <summary>
    /// Message text to send to the user.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Emoji to display on the message.
    /// </summary>
    public string? Emoji { get; init; }

    /// <summary>
    /// Keyboard buttons to display (optional).
    /// </summary>
    public Collection<ButtonDto>? Keyboard { get; init; }

    /// <summary>
    /// Indicates whether this is the final step of a scenario.
    /// </summary>
    public bool IsFinal { get; init; }

    /// <summary>
    /// Indicates whether to send as a new message or edit existing.
    /// </summary>
    public bool IsNewMessage { get; init; }

    /// <summary>
    /// Notification text to show in a toast (callback answer).
    /// </summary>
    public string? Notification { get; init; }

    /// <summary>Creates an error result.</summary>
    public static CommandResult Error(string message)
        => new()
        {
            Message = $"❌ {message}",
            IsFinal = true,
            IsNewMessage = true
        };
}
