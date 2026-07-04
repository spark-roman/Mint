using Mint.Common.Contracts.Bot.Commands;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <summary>
/// Factory for create specific command handler
/// </summary>
public interface ICommandHandlerFactory
{
    /// <summary>
    /// Create specific command handler
    /// </summary>
    /// <param name="commandType">Tg command type</param>
    /// <returns>Specific command handler</returns>
    ICommandHandler Create(TgCommandType commandType);
}