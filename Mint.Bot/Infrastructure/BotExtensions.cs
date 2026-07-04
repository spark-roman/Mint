using Mint.App.Services.System.Bot.Handlers;
using Mint.Bot.Polling.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Mint.Bot.Infrastructure;

/// <summary>
/// Bot DI extension methods.
/// </summary>
public static class BotExtensions
{
    /// <summary>
    /// Registers bot services.
    /// </summary>
    /// <param name="builder">Web application builder</param>
    public static void RegisterTgBotServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var token = builder.Configuration["Auth:Tg:Token"] ?? throw new InvalidOperationException("Telegram bot token is not set.");
        var botId = builder.Configuration["Auth:Tg:BotId"] ?? throw new InvalidOperationException("Telegram bot id is not set.");

        builder.Services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient($"{botId}:{token}"));
        builder.Services.AddSingleton<IUpdateHandler, UpdateHandler>();
        builder.Services.AddHostedService<PollingService>();   
    }
}
