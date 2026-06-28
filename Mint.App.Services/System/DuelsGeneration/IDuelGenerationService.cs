using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.System.DuelsGeneration;

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
}
