using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.Common.Contracts.Bot.Commands;

namespace Mint.App.Services.Infrastructure.DI.System.Bot;

/// <summary>
/// DI extension methods for bot commands
/// </summary>
public static class BotCommandExtensions
{
    /// <summary>
    /// Register bot services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void RegisterBotServices(this IServiceCollection services)
    {
        services.AddKeyedScoped<ICommandHandler, StartCommandHandler>(TgCommandType.Start);
        services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();

        services.AddStartCommandMappers();
    }
}
