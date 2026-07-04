using Mint.App.Services.System.Bot.Dto;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <summary>
/// Defines a handler for text commands (e.g., /start, /profile).
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Handles a text command from the user.
    /// </summary>
    /// <param name="tgUser">Telegram user.</param>
    /// <param name="command">Command name (start, profile, duels, referral).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Command result containing message and keyboard.</returns>
    Task<CommandResult> HandleAsync(User tgUser, string command, CancellationToken cancellationToken);
}
