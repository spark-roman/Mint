namespace Mint.Common.Contracts.Bot.Commands;

/// <summary>
/// Telgram handled commands
/// </summary>
public enum TgCommandType
{
    /// <summary>
    /// For test only command
    /// </summary>
    ForTestOnly = -100500,

    /// <summary>
    /// Not supported command
    /// </summary>
    None = 0,

    /// <summary>
    /// Start command
    /// </summary>
    Start = 1,

    /// <summary>
    /// Add command
    /// </summary>
    Add = 2,

    /// <summary>
    /// List command
    /// </summary>
    List = 3,

    /// <summary>
    /// Delete command
    /// </summary>
    Delete = 4,

    /// <summary>
    /// Buy command
    /// </summary>
    Buy = 5,

    /// <summary>
    /// Help command
    /// </summary>
    Help = 6,

    /// <summary>
    /// Feedback command
    /// </summary>
    Feedback = 7
}
