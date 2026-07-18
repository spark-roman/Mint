using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Dto;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Mint.App.Services.System.Bot.Handlers.Messages;

/// <inheritdoc cref="IBotMessageSender"/>
public sealed class BotMessageSender(ITelegramBotClient botClient, ILogger<BotMessageSender> logger) : IBotMessageSender
{
    private readonly ITelegramBotClient _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));
    private readonly ILogger<BotMessageSender> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task SendMessageAsync(long chatId, string text, ParseMode parseMode, InlineKeyboardMarkup? replyMarkup, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.SendMessage(
                chatId: chatId,
                text: text,
                parseMode: parseMode,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            _logger.LogError(ex, "Failed to send message to chat {ChatId}", chatId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task EditMessageAsync(long chatId, int messageId, string text, ParseMode parseMode, InlineKeyboardMarkup? replyMarkup, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.EditMessageText(
                chatId: chatId,
                messageId: messageId,
                text: text,
                parseMode: parseMode,
                replyMarkup: replyMarkup,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            _logger.LogError(ex, "Failed to edit message {MessageId} in chat {ChatId}", messageId, chatId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task SendOrEditMessageAsync(UpdateCommandDto command, CommandResult commandResult, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);
        ArgumentNullException.ThrowIfNull(commandResult);

        if (commandResult.IsNewMessage)
        {
            await SendMessageAsync(
                command.ChatId,
                command.CommandText!,
                ParseMode.Markdown,
                BuildKeyboard(commandResult.Keyboard),
                cancellationToken);
        }
        else
        {
            await EditMessageAsync(
                command.ChatId,
                command.MessageId,
                command.CommandText!,
                ParseMode.Markdown,
                BuildKeyboard(commandResult.Keyboard),
                cancellationToken);

            if (!string.IsNullOrEmpty(commandResult.Emoji))
            {
                await SendReactionAsync(
                    command.ChatId,
                    command.MessageId,
                    commandResult.Emoji,
                    cancellationToken);
            }
        }
    }

    private static InlineKeyboardMarkup? BuildKeyboard(IReadOnlyCollection<ButtonDto>? buttons)
    {
        if (buttons == null || buttons.Count == 0)
            return null;

        var rows = buttons
            .OrderBy(b => b.OrderNum)
            .Select(b => new[]
            {
                InlineKeyboardButton.WithCallbackData(b.Caption, b.Action)
            })
            .ToArray();

        return new InlineKeyboardMarkup(rows);
    }

    /// <inheritdoc />
    public async Task SendReactionAsync(long chatId, int messageId, string emoji, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.SetMessageReaction(
                chatId: chatId,
                messageId: messageId,
                reaction: [new ReactionTypeEmoji { Emoji = emoji }],
                isBig: true,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to set reaction {Emoji} on message {MessageId}", emoji, messageId);
        }
    }

    /// <inheritdoc />
    public async Task AnswerCallbackAsync(string callbackId, string? text, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.AnswerCallbackQuery(callbackQueryId: callbackId, text: text, cancellationToken: cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to answer callback {CallbackId}", callbackId);
        }
    }
}
