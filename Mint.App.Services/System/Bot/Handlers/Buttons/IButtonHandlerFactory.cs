using Mint.Common.Contracts.Bot.Commands;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <summary>
/// Factory for creating specific button handler.
/// </summary>
public interface IButtonHandlerFactory
{
    /// <summary>
    /// Creates a specific button handler.
    /// </summary>
    /// <param name="commandType">Telegram command type.</param>
    /// <returns>Specific button handler.</returns>
    IButtonHandler? Create(TgCommandType commandType);
}
