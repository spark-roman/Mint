using Mint.App.Services.System.DuelsGeneration.Dto;

namespace Mint.App.Services.System.DuelsGeneration.GigaChat;

/// <summary>
/// Service for interacting with GigaChat API
/// </summary>
public interface IGigaChatService
{
    /// <summary>
    /// Generate duels from a user prompt
    /// </summary>
    /// <param name="userPrompt">User prompt content</param>
    /// <param name="count">Expected number of duels</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of generated duels</returns>
    Task<List<DuelGenerationDto>> GenerateDuelsAsync(string userPrompt, int count, CancellationToken cancellationToken);
}
