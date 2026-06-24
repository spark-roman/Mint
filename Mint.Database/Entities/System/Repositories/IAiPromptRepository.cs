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
    /// <returns>AI prompt DTO or null if not found</returns>
    Task<AiPromptDto?> GetAsync(CancellationToken cancellationToken);
}
