using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Mappers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Mint.App.Services.System.Bot.Handlers;

/// <inheritdoc/>
public class UpdateHandler(
    IServiceScopeFactory serviceScopeFactory,
    IDtoMapper<Update, UpdateCommandDto> userMapper,
    ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory
        ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    private readonly IDtoMapper<Update, UpdateCommandDto> _userMapper = userMapper
        ?? throw new ArgumentNullException(nameof(userMapper));

    private readonly ILogger<UpdateHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Polling error: {exception?.Message}");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);

        _logger.LogInformation("Update command from tg:{Data}", update);

        if (update.Message is null && update.CallbackQuery is null)
        {
            _logger.LogError("Message and callback is null");

            return;
        }

        var updateCommand = _userMapper.Map(update);

        if (!string.IsNullOrEmpty(updateCommand.CallbackId))
        {
            try
            {
                await botClient.AnswerCallbackQueryAsync(update.CallbackQuery!.Id, cancellationToken: cancellationToken);
            }
            catch (ApiRequestException ex)
            {
                _logger.LogError(ex, "Error while answering callback query");
            }
        }

#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            var scope = _serviceScopeFactory.CreateScope();
            var commandHandlerFactory = scope.ServiceProvider.GetRequiredService<ICommandHandlerFactory>();

            var handler = commandHandlerFactory.Create(TgCommandType.Start);

            var commandResult = await handler.HandleAsync(updateCommand.User!, "start", cancellationToken);

            await botClient.SendTextMessageAsync(
                chatId: updateCommand.ChatId,
                text: commandResult.Message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: commandResult.Keyboard is not null
                    ? new InlineKeyboardMarkup(
                        [
                            ..commandResult.Keyboard.Select(button => new[] { InlineKeyboardButton.WithCallbackData(button.Caption, button.Action) })
                        ])
                    : null,
                cancellationToken: cancellationToken);
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling update");
        }
        finally
        {
            if (GC.GetTotalMemory(false) >= 125 * 1024 * 1024)
            {
                _logger.LogDebug("High memory usage detected, suggesting GC");
                GC.Collect();
            }
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}
