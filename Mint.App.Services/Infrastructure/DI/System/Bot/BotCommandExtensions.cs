using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Buttons;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.System.Bot.Handlers.Router;
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
        services.AddScoped<ICommandRouter, CommandRouter>();

        services.AddSingleton<IMessageFormatter, MessageFormatter>();

        services.AddKeyedScoped<ICommandHandler, DuelsCommandHandler>(TgCommandType.Duels);
        services.AddKeyedScoped<ICommandHandler, StartCommandHandler>(TgCommandType.Start);
        services.AddKeyedScoped<ICommandHandler, ProfileCommandHandler>(TgCommandType.Profile);
        services.AddKeyedScoped<ICommandHandler, ClaimBonusCommandHandler>(TgCommandType.ClaimBonus);
        services.AddKeyedScoped<ICommandHandler, MainMenuCommandHandler>(TgCommandType.MainMenu);
        services.AddKeyedScoped<ICommandHandler, BonusUnavailableHandler>(TgCommandType.BonusUnavailable);
        services.AddKeyedScoped<ICommandHandler, LeaderboardCommandHandler>(TgCommandType.Leaderboard);

        services.AddKeyedScoped<IButtonHandler, ButtonClickHandler>(TgCommandType.CategorySelection);
        services.AddKeyedScoped<IButtonHandler, ButtonClickHandler>(TgCommandType.DuelSelection);
        services.AddKeyedScoped<IButtonHandler, ButtonClickHandler>(TgCommandType.Vote);
        services.AddKeyedScoped<IButtonHandler, ButtonClickHandler>(TgCommandType.BetPlacement);
        services.AddKeyedScoped<IButtonHandler, ButtonClickHandler>(TgCommandType.Cancel);
        services.AddKeyedScoped<IButtonHandler, ButtonClickHandler>(TgCommandType.Share);
        services.AddKeyedScoped<IButtonHandler, ButtonClickHandler>(TgCommandType.CallbackNavigation);

        services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
        services.AddScoped<IButtonHandlerFactory, ButtonHandlerFactory>();

        services.AddStartCommandMappers();
    }
}
