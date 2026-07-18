using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Mint.App.Services.System.Bot.Handlers.Messages;

/// <summary>
/// Provides methods for sending and editing messages in Telegram.
/// </summary>
public interface IBotMessageSender
{
    /// <summary>
    /// Sends a new message to the user.
    /// </summary>
    Task SendMessageAsync(
        long chatId,
        string text,
        ParseMode parseMode,
        InlineKeyboardMarkup? replyMarkup,
        CancellationToken cancellationToken);

    /// <summary>
    /// Edits an existing message.
    /// </summary>
    Task EditMessageAsync(
        long chatId,
        int messageId,
        string text,
        ParseMode parseMode,
        InlineKeyboardMarkup? replyMarkup,
        CancellationToken cancellationToken);

    /// <summary>
    /// Sends a message or edits an existing message.
    /// </summary>
    /// <param name="command">Bot command.</param>
    /// <param name="commandResult">Bot command result.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendOrEditMessageAsync(UpdateCommandDto command, CommandResult commandResult, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a reaction to a message.
    /// </summary>
    Task SendReactionAsync(
        long chatId,
        int messageId,
        string emoji,
        CancellationToken cancellationToken);

    /// <summary>
    /// Answers a callback query.
    /// </summary>
    Task AnswerCallbackAsync(
        string callbackId,
        string? text,
        CancellationToken cancellationToken);
}
