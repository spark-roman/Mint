namespace Mint.Database.Entities.UserInteractive.Duels.Dto;

/// <summary>
/// DTO for creating a duel option
/// </summary>
public record DuelOptionCreateDto
{
    /// <summary>
    /// Text displayed on the button
    /// </summary>
    public required string OptionText { get; init; }

    /// <summary>
    /// Short code for the option (e.g. "up", "hold")
    /// </summary>
    public required string OptionCode { get; init; }
}
