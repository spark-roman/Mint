using System.Collections.ObjectModel;
using System.Globalization;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.UserInteractive.Duels.Dto;

/// <summary>
/// Data for rendering a duel card with options and expiration time.
/// </summary>
public sealed record DuelCardDto
{
    /// <summary>
    /// Unique identifier of the duel.
    /// </summary>
    public required long DuelId { get; init; }

    /// <summary>
    /// Name of the duel category.
    /// </summary>
    public required string CategoryName { get; init; }

    /// <summary>
    /// The duel question or proposition to bet on.
    /// </summary>
    public required string Question { get; init; }

    /// <summary>
    /// Additional description or context for the duel.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// UTC timestamp when the duel expires.
    /// </summary>
    public required DateTimeOffset ExpiresAt { get; init; }

    /// <summary>
    /// List of available betting options for this duel.
    /// </summary>
    public required Collection<DuelOptionDto> Options { get; init; } = null!;

    /// <summary>
    /// Formatted time remaining until duel expiration in hh:mm:ss format.
    /// </summary>
    public string TimeLeft => (ExpiresAt - DateTimeOffset.UtcNow).ToString(@"hh\:mm\:ss", CultureInfo.InvariantCulture);
}
