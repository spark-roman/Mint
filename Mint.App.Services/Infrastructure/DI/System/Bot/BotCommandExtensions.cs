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
        services.AddSingleton<IBotMessageSender, BotMessageSender>();

        services.AddKeyedScoped<ICommandHandler, DuelsCommandHandler>(TgCommandType.Duels);
        services.AddKeyedScoped<ICommandHandler, StartCommandHandler>(TgCommandType.Start);
        services.AddKeyedScoped<ICommandHandler, ProfileCommandHandler>(TgCommandType.Profile);
        services.AddKeyedScoped<ICommandHandler, ClaimBonusCommandHandler>(TgCommandType.ClaimBonus);
        services.AddKeyedScoped<ICommandHandler, MainMenuCommandHandler>(TgCommandType.MainMenu);
        services.AddKeyedScoped<ICommandHandler, BonusUnavailableHandler>(TgCommandType.BonusUnavailable);
        services.AddKeyedScoped<ICommandHandler, LeaderboardCommandHandler>(TgCommandType.Leaderboard);
        services.AddKeyedScoped<ICommandHandler, HelpCommandHandler>(TgCommandType.Help);
        services.AddKeyedScoped<ICommandHandler, NumberInputCommandHandler>(TgCommandType.NumberInput);
        services.AddKeyedScoped<ICommandHandler, ReferralCommandHandler>(TgCommandType.Referral);

        services.AddKeyedScoped<IButtonHandler, NavigationButtonHandler>(TgCommandType.MainMenu);
        services.AddKeyedScoped<IButtonHandler, NavigationButtonHandler>(TgCommandType.Profile);
        services.AddKeyedScoped<IButtonHandler, NavigationButtonHandler>(TgCommandType.Duels);
        services.AddKeyedScoped<IButtonHandler, NavigationButtonHandler>(TgCommandType.Referral);
        services.AddKeyedScoped<IButtonHandler, NavigationButtonHandler>(TgCommandType.Leaderboard);
        services.AddKeyedScoped<IButtonHandler, CategorySelectionButtonHandler>(TgCommandType.CategorySelection);
        services.AddKeyedScoped<IButtonHandler, VoteButtonHandler>(TgCommandType.Vote);
        services.AddKeyedScoped<IButtonHandler, BetButtonHandler>(TgCommandType.BetPlacement);
        services.AddKeyedScoped<IButtonHandler, CancelButtonHandler>(TgCommandType.Cancel);
        services.AddKeyedScoped<IButtonHandler, BonusButtonHandler>(TgCommandType.ClaimBonus);
        services.AddKeyedScoped<IButtonHandler, ShareButtonHandler>(TgCommandType.Share);
        services.AddKeyedScoped<IButtonHandler, CallbackNavigationButtonHandler>(TgCommandType.CallbackNavigation);

        services.AddScoped<ICommandHandlerFactory, CommandHandlerFactory>();
        services.AddScoped<IButtonHandlerFactory, ButtonHandlerFactory>();

        services.AddStartCommandMappers();
    }
}
