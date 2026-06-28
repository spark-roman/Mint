namespace Mint.Database.Entities.UserInteractive.Stats.Dto;

/// <summary>
/// DTO for rank config
/// </summary>
public record RankConfigDto
{
    /// <summary>
    /// Rank config ID
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Rank code
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Rank name
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Rank emoji
    /// </summary>
    public required string Emoji { get; init; }

    /// <summary>
    /// Minimum points required
    /// </summary>
    public int MinPoints { get; init; }
}
