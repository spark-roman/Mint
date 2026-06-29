using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Mint.Bot.Polling.Services;

/// <summary>
/// Background service for polling
/// </summary>
public class PollingService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;
    private readonly ILogger<PollingService> _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="botClient">Telegram bot client</param>
    /// <param name="updateHandler">Bot command handler</param>
    /// <param name="logger">Logger</param>
    public PollingService(
        ITelegramBotClient botClient,
        IUpdateHandler updateHandler,
        ILogger<PollingService> logger)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
        _logger = logger;
    }

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
