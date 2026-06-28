using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Processors;

/// <summary>
/// Processes a string response from AI to create duels.
/// </summary>
public interface IAIResponseProcessor
{
    /// <summary>
    /// Processes an AI response into a list of duel creation DTOs.
    /// </summary>
    /// <param name="response">The AI response.</param>
    /// <param name="categoryId">The category ID for the duels.</param>
    /// <param name="daysToLive">Number of days the duels should live.</param>
    /// <returns>A list of duel creation DTOs.</returns>
    Task<List<DuelCreateDto>> Process(string response, int categoryId, int daysToLive);
}
