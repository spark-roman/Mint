namespace Mint.App.Services.UserInteractive.Profiles.Dto;

/// <summary>
/// Leaderboard entry
/// </summary>
public record LeaderboardEntryDto
{
    /// <summary>Rank position (1-based).</summary>
    public required int Rank { get; init; }

    /// <summary>User's external ID (Telegram).</summary>
    public required long ExternalUserId { get; init; }

    /// <summary>User's display name.</summary>
    public required string DisplayName { get; init; }

    /// <summary>User's rank name with emoji.</summary>
    public required string RankName { get; init; }

    /// <summary>Rank points.</summary>
    public required decimal RankPoints { get; init; }
}
