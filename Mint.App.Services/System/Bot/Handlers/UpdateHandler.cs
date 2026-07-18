using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.System.Bot.Handlers.Router;
using Mint.Common.Contracts.Mappers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers;

/// <summary>
/// Handles incoming updates from Telegram.
/// </summary>
public class UpdateHandler(
    IServiceScopeFactory serviceScopeFactory,
    IDtoMapper<Update, UpdateCommandDto> commandMapper,
    ILogger<UpdateHandler> logger) : IUpdateHandler
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory
        ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

    private readonly IDtoMapper<Update, UpdateCommandDto> _commandMapper = commandMapper
        ?? throw new ArgumentNullException(nameof(commandMapper));

    private readonly ILogger<UpdateHandler> _logger = logger 
        ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(update);

        _logger.LogInformation("Update received: {UpdateType}", update.Type);

        var updateCommand = _commandMapper.Map(update);

        await using var scope = _serviceScopeFactory.CreateAsyncScope();
        
        var messageSender = scope.ServiceProvider.GetRequiredService<IBotMessageSender>();
        var router = scope.ServiceProvider.GetRequiredService<ICommandRouter>();

        if (!string.IsNullOrEmpty(updateCommand.CallbackId))
        {
            await messageSender.AnswerCallbackAsync(updateCommand.CallbackId, null, cancellationToken);
        }

        try
        {
            var commandResult = await router.RouteAsync(updateCommand, cancellationToken);

            if (!string.IsNullOrEmpty(commandResult.Notification))
            {
                await messageSender.AnswerCallbackAsync(
                    updateCommand.CallbackId!,
                    commandResult.Notification,
                    cancellationToken);
            }

            if (string.IsNullOrEmpty(commandResult.Message))
            {
                _logger.LogInformation("Empty message, skipping send");
                return;
            }

            await messageSender.SendOrEditMessageAsync(updateCommand, commandResult, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while handling update");
            throw;
        }
    }

    /// <inheritdoc/>
    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Polling error");
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Polling error");
        return Task.CompletedTask;
    }
}