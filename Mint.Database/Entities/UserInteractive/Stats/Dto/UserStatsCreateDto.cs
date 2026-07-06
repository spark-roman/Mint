namespace Mint.Database.Entities.UserInteractive.Stats.Dto;

/// <summary>
/// DTO for creating user stats
/// </summary>
public record UserStatsCreateDto
{
    /// <summary>
    /// User id
    /// </summary>
    public required long ExternalUserId { get; init; }

    /// <summary>
    /// Internal user id
    /// </summary>
    public long InternalUserId { get; set; }

    /// <summary>
    /// Rank points
    /// </summary>
    public decimal RankPoints { get; init; }

    /// <summary>
    /// Total wins
    /// </summary>
    public int TotalWins { get; init; }

    /// <summary>
    /// Total losses
    /// </summary>
    public int TotalLosses { get; init; }
}
