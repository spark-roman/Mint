using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.App.Services.System.Bot.Handlers.Router;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Bot.Commands.Dto;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Mint.App.Services.System.Bot.Handlers;

/// <summary>
/// Handles incoming updates from Telegram.
/// </summary>
public class UpdateHandler(
    IServiceScopeFactory serviceScopeFactory,
    IDtoMapper<Update, UpdateCommandDto> userMapper,
    ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory
        ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    private readonly IDtoMapper<Update, UpdateCommandDto> _userMapper = userMapper
        ?? throw new ArgumentNullException(nameof(userMapper));

    private readonly ILogger<UpdateHandler> _logger = logger 
        ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Polling error");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);

        _logger.LogInformation("Update received: {UpdateType}", update.Type);

        var updateCommand = _userMapper.Map(update);

        if (!string.IsNullOrEmpty(updateCommand.CallbackId))
        {
            await AnswerCallbackAsync(botClient, updateCommand.CallbackId, cancellationToken);
        }
        
        #pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            await using var scope = _serviceScopeFactory.CreateAsyncScope();
            var router = scope.ServiceProvider.GetRequiredService<ICommandRouter>();
            var commandResult = await router.RouteAsync(updateCommand, cancellationToken);

            if (!string.IsNullOrEmpty(commandResult.Notification))
            {
                await botClient.AnswerCallbackQueryAsync(
                    updateCommand.CallbackId!,
                    commandResult.Notification,
                    cancellationToken: cancellationToken);
            }

            if (string.IsNullOrEmpty(commandResult.Message))
            {
                _logger.LogInformation("Empty message, skipping send");
                return;
            }

            await SendResponseAsync(botClient, updateCommand, commandResult, cancellationToken);
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling update");
        }
        #pragma warning restore CA1031 // Do not catch general exception types  
    }

    private async Task AnswerCallbackAsync(ITelegramBotClient botClient, string callbackId, CancellationToken ct)
    {
        try
        {
            await botClient.AnswerCallbackQueryAsync(callbackId, cancellationToken: ct);
        }
        catch (ApiRequestException ex)
        {
            _logger.LogError("Error while answering callback query: {CallbackId}, message: {Message}", callbackId, ex.Message);
        }
    }

    private async Task SendResponseAsync(
        ITelegramBotClient botClient, 
        UpdateCommandDto updateCommand, 
        CommandResult result, 
        CancellationToken ct)
    {
        try
        {
            if (result.IsNewMessage)
            {
                await botClient.SendTextMessageAsync(
                    chatId: updateCommand.ChatId,
                    text: result.Message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: BuildKeyboard(result.Keyboard),
                    cancellationToken: ct);
            }
            else
            {
                await botClient.EditMessageTextAsync(
                    chatId: updateCommand.ChatId,
                    messageId: updateCommand.MessageId,
                    text: result.Message,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: BuildKeyboard(result.Keyboard),
                    cancellationToken: ct);
            }
        }
        catch (ApiRequestException ex)
        {
            _logger.LogError(ex, "Error sending message to user {UserId}", updateCommand.User?.Id);
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
}
