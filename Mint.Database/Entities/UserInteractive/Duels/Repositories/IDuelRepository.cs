using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.Database.Entities.UserInteractive.Duels.Repositories;

/// <summary>
/// Repository interface for working with duels
/// </summary>
public interface IDuelRepository
{
    /// <summary>
    /// Create a new duel
    /// </summary>
    /// <param name="dto">DTO for creation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ID of the created duel</returns>
    Task<long> CreateDuelAsync(DuelCreateDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Get duel by ID
    /// </summary>
    /// <param name="duelId">Duel ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Duel DTO or null if not found</returns>
    Task<DuelDto?> GetDuelByIdAsync(long duelId, CancellationToken cancellationToken);

    /// <summary>
    /// Get active duels (not closed and not expired)
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active duels</returns>
    Task<List<DuelDto>?> GetActiveDuelsAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get first available duel for category
    /// </summary>
    /// <param name="categoryId">Category id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Duel DTO or null if not found</returns>
    Task<DuelDto?> GetFirstAvailableDuelAsync(int categoryId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a duel option by its identifier.
    /// </summary>
    /// <param name="optionId">Duel option id</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Duel option DTO or null if not found</returns>
    
    Task<DuelOptionDto?> GetOptionByIdAsync(long optionId, CancellationToken cancellationToken);
}
