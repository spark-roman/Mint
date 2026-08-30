namespace Mint.Common.Contracts.Bot.Commands;

/// <summary>
/// Button type.
/// </summary>
public enum TgButtonType
{
    /// <summary>
    /// None.
    /// </summary>
    None = 0,

    /// <summary>
    /// Callback data.
    /// </summary>
    CallbackData = 1,

    /// <summary>
    /// Switch inline query.
    /// </summary>
    SwitchInlineQuery = 2,
}
