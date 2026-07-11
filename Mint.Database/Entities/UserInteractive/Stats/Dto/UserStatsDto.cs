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
    /// User external id
    /// </summary>
    public long ExternalUserId { get; set; }

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

    /// <summary>
    /// Number of successfully referred friends who completed the conditions
    /// </summary>
    public int ReferralCount { get; init; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; }

    /// <summary>
    /// User name
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Rank name
    /// </summary>
    public string RankName { get; set; } = string.Empty;   
}

/// <summary>
/// DTO for updating user stats
/// </summary>
public record UserStatsUpdateDto
{
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

    /// <summary>
    /// Number of successfully referred friends who completed the conditions
    /// </summary>
    public int ReferralCount { get; init; }
}

