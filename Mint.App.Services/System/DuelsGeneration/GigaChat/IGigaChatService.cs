using Mint.App.Services.System.DuelsGeneration.Dto;

namespace Mint.App.Services.System.DuelsGeneration.GigaChat;

/// <summary>
/// Service for interacting with GigaChat API
/// </summary>
public interface IGigaChatService
{
    /// <summary>
    /// Generate duels from system and user prompts
    /// </summary>
    /// <param name="systemPrompt">System prompt with instructions</param>
    /// <param name="userPrompt">User prompt with content</param>
    /// <param name="count">Expected number of duels</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of generated duels</returns>
    Task<List<DuelGenerationDto>> GenerateDuelsAsync(string systemPrompt, string userPrompt, int count, CancellationToken cancellationToken);
}
