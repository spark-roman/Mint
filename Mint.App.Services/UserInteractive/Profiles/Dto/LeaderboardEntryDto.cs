namespace Mint.App.Services.UserInteractive.Profiles.Dto;

/// <summary>
/// Leaderboard entry
/// </summary>
public record LeaderboardEntryDto
{
    /// <summary>
    /// Rank of the user in the leaderboard
    /// </summary>
    public int Rank { get; init; }

    /// <summary>
    /// User's external ID
    /// </summary>
    public long ExternalUserId { get; init; }

    /// <summary>
    /// User's display name
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Points earned for being in a certain rank
    /// </summary>
    public decimal RankPoints { get; init; }

    /// <summary>
    /// Total number of duels won by the user
    /// </summary>
    public int TotalDuels { get; init; }

    /// <summary>
    /// Win rate of the user
    /// </summary>
    public double Winrate { get; init; }
}
