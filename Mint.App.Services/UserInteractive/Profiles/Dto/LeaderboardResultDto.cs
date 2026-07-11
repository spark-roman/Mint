namespace Mint.App.Services.UserInteractive.Profiles.Dto;

/// <summary>
/// Result of leaderboard request.
/// </summary>
public sealed record LeaderboardResultDto
{
    /// <summary>List of top entries.</summary>
    public required IReadOnlyCollection<LeaderboardEntryDto> Entries { get; init; }

    /// <summary>Total number of users.</summary>
    public required int TotalUsers { get; init; }

    /// <summary>User's own rank (null if not found).</summary>
    public int? UserRank { get; init; }

    /// <summary>User's own entry (null if not found).</summary>
    public LeaderboardEntryDto? UserEntry { get; init; }
}
