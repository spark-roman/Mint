using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;

namespace Mint.App.Services.System.Bot.Handlers.Router;

/// <summary>
/// Responsible for routing incoming updates to appropriate command handlers.
/// </summary>
public interface ICommandRouter
{
    /// <summary>
    /// Routes an incoming update to the appropriate handler and returns the result.
    /// </summary>
    /// <param name="updateCommand">Parsed update command data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Command result containing message and keyboard.</returns>
    Task<CommandResult> RouteAsync(UpdateCommandDto updateCommand, CancellationToken cancellationToken);
}
