using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <summary>
/// Defines a handler for button clicks (callback queries).
/// </summary>
public interface IButtonHandler
{
    /// <summary>
    /// Handles a button click from the user.
    /// </summary>
    /// <param name="externalUserId">Telegram user identifier.</param>
    /// <param name="callbackData">Button action identifier.</param>
    /// <param name="updateCommand">Update command.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Command result containing message and keyboard.</returns>
    Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken);
}
