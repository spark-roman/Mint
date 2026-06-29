using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace AdvBot.Bot.Handlers;

/// <inheritdoc/>
public class UpdateHandler(ILogger<UpdateHandler> logger) : IUpdateHandler
{
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

        if (!string.IsNullOrEmpty(update.CallbackQuery!.Id))
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
