using System.Globalization;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Buttons;
using Mint.App.Services.System.Bot.Handlers.Commands;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.Common.Contracts.Bot.Commands;
using Telegram.Bot.Types.Enums;

namespace Mint.App.Services.System.Bot.Handlers.Router;

/// <inheritdoc/>
public sealed class CommandRouter(
    ICommandHandlerFactory handlerFactory,
    IButtonHandlerFactory buttonHandlerFactory,
    ILogger<CommandRouter> logger) : ICommandRouter
{
    private readonly ICommandHandlerFactory _handlerFactory = handlerFactory ?? throw new ArgumentNullException(nameof(handlerFactory));

    private readonly IButtonHandlerFactory _buttonHandlerFactory = buttonHandlerFactory ?? throw new ArgumentNullException(nameof(buttonHandlerFactory));

    private readonly ILogger<CommandRouter> _logger = logger;

    /// <inheritdoc/>
    public async Task<CommandResult> RouteAsync(UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(updateCommand);

        var commandType = DetermineCommandType(updateCommand);
        _logger.LogInformation("Routing command type: {CommandType} for user {UserId}", commandType, updateCommand.User?.Id);

        var commandHandler = _handlerFactory.Create(commandType);

        if (commandHandler != null)
        {
            var inputData = ExtractInputData(updateCommand);

            var result = await commandHandler.HandleAsync(updateCommand.User!, inputData, cancellationToken);

            _logger.LogInformation("Command handled: {CommandType}, IsFinal: {IsFinal}", commandType, result.IsFinal);

            return result;
        }

        var buttonHandler = _buttonHandlerFactory.Create(commandType);

        if (buttonHandler != null)
        {
            var inputData = ExtractInputData(updateCommand);

            var result = await buttonHandler.HandleAsync(updateCommand.User!.Id, inputData, cancellationToken);

            _logger.LogInformation("Button handled: {CommandType}, IsFinal: {IsFinal}", commandType, result.IsFinal);

            return result;
        }

        throw new InvalidOperationException($"Unknown command type: {commandType}");
    }

    /// <summary>
    /// Determines the command type based on the incoming update.
    /// </summary>
private TgCommandType DetermineCommandType(UpdateCommandDto updateCommand)
{
    // === Обработка Callback-запросов ===
    if (!string.IsNullOrEmpty(updateCommand.CallbackData))
    {
        _logger.LogInformation("Callback: {CallbackData}", updateCommand.CallbackData);

        return updateCommand.CallbackData switch
        {
            ActionConstants.MainMenu => TgCommandType.MainMenu,
            ActionConstants.Profile => TgCommandType.Profile,
            ActionConstants.Duels => TgCommandType.Duels,
            ActionConstants.Referral => TgCommandType.Referral,
            ActionConstants.BackToProfile => TgCommandType.BackToProfile,
            
            ActionConstants.ClaimBonus => TgCommandType.ClaimBonus,
            ActionConstants.BonusUnavailable => TgCommandType.BonusUnavailable,
            
            ActionConstants.Leaderboard => TgCommandType.Leaderboard,
            
            var action when action.StartsWith(ActionConstants.CategoryPrefix, StringComparison.InvariantCultureIgnoreCase) => TgCommandType.CategorySelection,
            var action when action.StartsWith(ActionConstants.VotePrefix, StringComparison.InvariantCultureIgnoreCase) => TgCommandType.Vote,
            var action when action.StartsWith(ActionConstants.BetPrefix, StringComparison.InvariantCultureIgnoreCase) => TgCommandType.BetPlacement,
            var action when action.StartsWith(ActionConstants.CancelPrefix, StringComparison.InvariantCultureIgnoreCase) => TgCommandType.Cancel,
            var action when action.StartsWith(ActionConstants.SharePrefix, StringComparison.InvariantCultureIgnoreCase) => TgCommandType.Share,
            
            _ => TgCommandType.CallbackNavigation
        };
    }

    if (!string.IsNullOrEmpty(updateCommand.CommandText))
    {
        _logger.LogInformation("CommandText: {CommandText}", updateCommand.CommandText);

        if (updateCommand.CommandText.StartsWith('/'))
        {
            return updateCommand.CommandText.ToUpperInvariant() switch
            {
                "/START" => TgCommandType.Start,
                "/HELP" => TgCommandType.Help,
                "/PROFILE" => TgCommandType.Profile,
                "/DUELS" => TgCommandType.Duels,
                "/REFERRAL" => TgCommandType.Referral,
                "/LEADERBOARD" => TgCommandType.Leaderboard,
                "/ADMIN" => TgCommandType.Admin,
                _ => TgCommandType.None
            };
        }

        if (decimal.TryParse(updateCommand.CommandText, out _))
        {
            return TgCommandType.NumberInput;
        }

        return TgCommandType.TextInput;
    }

    return TgCommandType.None;
}

    /// <summary>
    /// Extracts input data from the update.
    /// </summary>
    private static string ExtractInputData(UpdateCommandDto updateCommand)
    {
        return updateCommand.Type == UpdateType.CallbackQuery
            ? updateCommand!.CallbackData!
            : updateCommand!.CommandText!;
    }
}
