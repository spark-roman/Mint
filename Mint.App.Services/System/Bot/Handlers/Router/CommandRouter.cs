using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.Common.Contracts.Bot.Commands;

namespace Mint.App.Services.System.Bot.Handlers.Router;

/// <inheritdoc/>
public sealed class CommandRouter(
    ICommandHandlerFactory handlerFactory,
    ILogger<CommandRouter> logger) : ICommandRouter
{
    private readonly ICommandHandlerFactory _handlerFactory = handlerFactory;
    private readonly ILogger<CommandRouter> _logger = logger;

    /// <inheritdoc/>
    public async Task<CommandResult> RouteAsync(UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(updateCommand);

        var commandType = DetermineCommandType(updateCommand);
        _logger.LogDebug("Routing command type: {CommandType} for user {UserId}", commandType, updateCommand.User?.Id);

        var handler = _handlerFactory.Create(commandType);

        var inputData = ExtractInputData(updateCommand);

        var result = await handler.HandleAsync(updateCommand.User!, inputData, cancellationToken);

        _logger.LogDebug("Command handled: {CommandType}, IsFinal: {IsFinal}", commandType, result.IsFinal);

        return result;
    }

    /// <summary>
    /// Determines the command type based on the incoming update.
    /// </summary>
    private static TgCommandType DetermineCommandType(UpdateCommandDto updateCommand)
    {
        if (!string.IsNullOrEmpty(updateCommand.CallbackData))
        {
            return updateCommand.CallbackData switch
            {
                "profile" => TgCommandType.Profile,
                "duels" => TgCommandType.Duels,
                "referral" => TgCommandType.Referral,
                "main_menu" => TgCommandType.MainMenu,
                "claim_bonus" => TgCommandType.ClaimBonus,
                "leaderboard" => TgCommandType.Leaderboard,
                var action when action.StartsWith("category_", StringComparison.InvariantCultureIgnoreCase) => TgCommandType.CategorySelection,
                var action when action.StartsWith("duel_", StringComparison.InvariantCultureIgnoreCase) => TgCommandType.DuelSelection,
                var action when action.StartsWith("bet_", StringComparison.InvariantCultureIgnoreCase) => TgCommandType.BetPlacement,
                var action when action.StartsWith("step_", StringComparison.InvariantCultureIgnoreCase) => TgCommandType.CallbackNavigation,
                _ => TgCommandType.Callback
            };
        }

        if (!string.IsNullOrEmpty(updateCommand.CommandText))
        {
            if (updateCommand.CommandText.StartsWith('/'))
            {
                return updateCommand.CommandText.ToUpperInvariant() switch
                {
                    "/START" => TgCommandType.Start,
                    "/PROFILE" => TgCommandType.Profile,
                    "/DUELS" => TgCommandType.Duels,
                    "/REFERRAL" => TgCommandType.Referral,
                    "/HELP" => TgCommandType.Help,
                    _ => TgCommandType.None
                };
            }

            if (decimal.TryParse(updateCommand.CommandText, out _))
            {
                return TgCommandType.TextInput;
            }

            return TgCommandType.None;
        }

        return TgCommandType.None;
    }

    /// <summary>
    /// Extracts input data from the update.
    /// </summary>
    private static string ExtractInputData(UpdateCommandDto updateCommand)
    {
        return updateCommand.CallbackData ?? updateCommand.CommandText ?? string.Empty;
    }
}
