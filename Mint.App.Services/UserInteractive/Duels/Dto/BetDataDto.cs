namespace Mint.App.Services.UserInteractive.Duels.Dto;

/// <summary>
/// Data for bet confirmation screen.
/// </summary>
public sealed record BetDataDto
{
    /// <summary>
    /// Unique identifier of the duel the bet is placed on.
    /// </summary>
    public required long DuelId { get; init; }

    /// <summary>
    /// Unique identifier of the selected option.
    /// </summary>
    public required long OptionId { get; init; }

    /// <summary>
    /// Display name of the selected option.
    /// </summary>
    public required string SelectedOption { get; init; }

    /// <summary>
    /// Current user balance available for betting.
    /// </summary>
    public required decimal Balance { get; init; }
}
