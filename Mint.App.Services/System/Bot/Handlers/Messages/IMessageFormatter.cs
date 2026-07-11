using Mint.App.Services.UserInteractive.Profiles.Dto;

namespace Mint.App.Services.System.Bot.Handlers.Messages;

/// <summary>
/// Formats messages by replacing placeholders with user data.
/// </summary>
public interface IMessageFormatter
{
    /// <summary>
    /// Formats a message template by replacing placeholders with user data.
    /// </summary>
    /// <param name="messageTemplate">Message template with placeholders like {{user_id}}.</param>
    /// <param name="userProfileDto">Use profile.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Formatted message string.</returns>
    Task<string> FormatAsync(string messageTemplate, UserProfileDto userProfileDto, CancellationToken cancellationToken);

    /// <summary>
    /// Formats a message template with leaderboard data.
    /// </summary>
    /// <param name="messageTemplate">Message template with placeholders like {{user_id}}.</param>
    /// <param name="leaderboardResult">Leader board.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Formatted message string.</returns>
    Task<string> FormatLeaderboardAsync(string messageTemplate, LeaderboardResultDto leaderboardResult, CancellationToken cancellationToken);
}
