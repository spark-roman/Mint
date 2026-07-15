using System.Globalization;

namespace Mint.App.Services.UserInteractive.Duels.Dto;

/// <summary>
/// Data for success screen after placing a bet.
/// </summary>
public sealed record SuccessDataDto
{
    /// <summary>
    /// Unique identifier of the duel the bet was placed on.
    /// </summary>
    public required long DuelId { get; init; }

    /// <summary>
    /// Display name of the selected betting option.
    /// </summary>
    public required string SelectedOption { get; init; }

    /// <summary>
    /// The amount of the bet placed.
    /// </summary>
    public required decimal BetAmount { get; init; }

    /// <summary>
    /// UTC timestamp when the duel expires.
    /// </summary>
    public required DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// Formatted time remaining until duel expiration in hh:mm:ss format.
    /// </summary>
    public string TimeLeft => (ExpiresAt - DateTimeOffset.UtcNow).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
}
