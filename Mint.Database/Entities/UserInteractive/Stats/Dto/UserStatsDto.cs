namespace Mint.Database.Entities.UserInteractive.Stats.Dto;

/// <summary>
/// DTO for user stats
/// </summary>
public record UserStatsDto
{
    /// <summary>
    /// Stats ID
    /// </summary>
    public long Id { get; init; }

    /// <summary>
    /// User ID
    /// </summary>
    public long UserId { get; init; }

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

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }
}

/// <summary>
/// DTO for updating user stats
/// </summary>
public record UserStatsUpdateDto
{
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
