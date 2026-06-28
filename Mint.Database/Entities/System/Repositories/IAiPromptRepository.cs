using Mint.Database.Entities.System.Dto;

namespace Mint.Database.Entities.System.Repositories;

/// <summary>
/// Repository interface for working with AI prompts
/// </summary>
public interface IAiPromptRepository
{
    /// <summary>
    /// Create or update AI prompt settings
    /// </summary>
    /// <param name="dto">DTO for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created/updated prompt</returns>
    Task<int> CreateOrUpdateAsync(AiPromptCreateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Get current AI prompt settings with active categories
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of AI prompt DTOs</returns>
    Task<List<AiPromptDto>> GetPromptsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get AI prompt settings by Id
    /// </summary>
    /// <param name="promptId">Prompt Id</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>AI prompt DTO</returns>
    Task<AiPromptDto?> GetPromptAsync(long promptId, CancellationToken cancellationToken);
}
