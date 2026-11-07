using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Services;

/// <summary>
/// Service for generating duels for users.
/// </summary>
public interface IDuelGenerationService
{
    /// <summary>
    /// Generates a list of duels for the given prompt.
    /// </summary>
    /// <param name="promptId">Prompt Id.</param>
    /// <param name="count">Count of duels to generate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of duels.</returns>
    Task<List<DuelCreateDto>> GenerateDuelsAsync(int promptId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Generates a list of duels for all active categories.
    /// </summary>
    /// <param name="maxPerCategory">Maximum number of duels per category.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of duels.</returns>
    Task<List<DuelCreateDto>> GenerateDuelsForAllActiveCategoriesAsync(int maxPerCategory, CancellationToken cancellationToken);

    /// <summary>
    /// Generates duels using GigaChat from active categories.
    /// </summary>
    /// <param name="promptId">Prompt Id.</param>
    /// <param name="count">Count of duels to generate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of duels.</returns>
    Task<List<DuelCreateDto>> GenerateDuelsWithGigaChatAsync(int promptId, int count, CancellationToken cancellationToken);

    /// <summary>
    /// Generates duels using GigaChat from unprocessed news articles.
    /// </summary>
    /// <param name="count">Count of duels to generate.</param>
    /// <param name="expiresAt">The exact expiration date/time for the duels.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of duels.</returns>
    Task<List<DuelCreateDto>> GenerateDuelsWithGigaChatFromNewsAsync(int count, DateTimeOffset expiresAt, CancellationToken cancellationToken);
}
