namespace Mint.Database.Entities.UserInteractive.Duels.Dto;

/// <summary>
/// DTO for duel option
/// </summary>
public record DuelOptionDto
{
    /// <summary>
    /// Option ID
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// Text displayed on the button
    /// </summary>
    public required string OptionText { get; init; }

    /// <summary>
    /// Short code for the option (e.g. "up", "hold")
    /// </summary>
    public required string OptionCode { get; init; }
}
