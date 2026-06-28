namespace Mint.Database.Entities.UserInteractive.Stats.Dto;

/// <summary>
/// DTO for creating user stats
/// </summary>
public record UserStatsCreateDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public required long UserId { get; init; }

    /// <summary>
    /// Rank points
    /// </summary>
    public int RankPoints { get; init; }

    /// <summary>
    /// Total wins
    /// </summary>
    public int TotalWins { get; init; }

    /// <summary>
    /// Total losses
    /// </summary>
    public int TotalLosses { get; init; }
}
