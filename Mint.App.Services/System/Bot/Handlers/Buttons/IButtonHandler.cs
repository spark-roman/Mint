using Mint.App.Services.System.Bot.Dto;

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
    /// <param name="cancellationToken"></param>
    /// <returns>Command result containing message and keyboard.</returns>
    Task<CommandResult> HandleAsync(long externalUserId, string callbackData, CancellationToken cancellationToken);
}
