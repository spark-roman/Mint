using System.Collections.ObjectModel;
using Mint.App.Services.UserInteractive.Duels.Dto;
using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.Bot.Handlers.Messages;

/// <summary>
/// Formats messages by replacing placeholders with structured data for bot responses.
/// </summary>
public interface IMessageFormatter
{
    /// <summary>
    /// Formats a message template with user profile data.
    /// </summary>
    /// <param name="messageTemplate">Message template containing placeholders to be replaced.</param>
    /// <param name="userProfileDto">User profile data containing values for placeholder replacement.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Formatted message string with all placeholders replaced.</returns>
    Task<string> FormatProfileAsync(string messageTemplate, UserProfileDto userProfileDto, CancellationToken cancellationToken);

    /// <summary>
    /// Formats a message template with leaderboard data.
    /// </summary>
    /// <param name="messageTemplate">Message template containing placeholders to be replaced.</param>
    /// <param name="leaderboardResult">Leaderboard data containing top entries and user rank information.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Formatted message string with leaderboard entries and user rank info.</returns>
    Task<string> FormatLeaderboardAsync(string messageTemplate, LeaderboardResultDto leaderboardResult, CancellationToken cancellationToken);

    /// <summary>
    /// Formats a message template with duel card data.
    /// </summary>
    /// <param name="messageTemplate">Message template containing placeholders to be replaced.</param>
    /// <param name="duelCard">Duel card data containing duel details, options, and expiration time.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Formatted message string with duel information and available options.</returns>
    Task<string> FormatDuelAsync(string messageTemplate, DuelCardDto duelCard, CancellationToken cancellationToken);

    /// <summary>
    /// Formats a message template with bet confirmation data.
    /// </summary>
    /// <param name="messageTemplate">Message template containing placeholders to be replaced.</param>
    /// <param name="betData">Bet data containing selected option, duel ID, and user balance information.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Formatted message string with bet confirmation details.</returns>
    Task<string> FormatBetAsync(string messageTemplate, BetDataDto betData, CancellationToken cancellationToken);

    /// <summary>
    /// Formats a message template with success result data.
    /// </summary>
    /// <param name="messageTemplate">Message template containing placeholders to be replaced.</param>
    /// <param name="successData">Success data containing the selected option, bet amount, and duel expiration time.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Formatted message string with bet success information and time remaining.</returns>
    Task<string> FormatSuccessAsync(string messageTemplate, SuccessDataDto successData, CancellationToken cancellationToken);

    /// <summary>
    /// Formats a message template with category selection options.
    /// </summary>
    /// <param name="messageTemplate">Message template containing placeholders to be replaced.</param>
    /// <param name="categories">Collection of available categories for selection.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>Formatted message string with category options listed.</returns>
    Task<string> FormatCategoriesAsync(string messageTemplate, Collection<CategoryDto> categories, CancellationToken cancellationToken);
}
