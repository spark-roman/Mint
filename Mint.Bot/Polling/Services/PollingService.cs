using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Mint.Bot.Polling.Services;

/// <summary>
/// Background service for polling
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="botClient">Telegram bot client</param>
/// <param name="updateHandler">Bot command handler</param>
/// <param name="logger">Logger</param>
public class PollingService(
    ITelegramBotClient botClient,
    IUpdateHandler updateHandler,
    ILogger<PollingService> logger) : BackgroundService
{
    private readonly ITelegramBotClient _botClient = botClient ?? throw new ArgumentNullException(nameof(botClient));

    private readonly IUpdateHandler _updateHandler = updateHandler ?? throw new ArgumentNullException(nameof(updateHandler));
    
    private readonly ILogger<PollingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting polling service");

        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = []
        };

        await _botClient.ReceiveAsync(
            updateHandler: _updateHandler,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken);
    }
}
