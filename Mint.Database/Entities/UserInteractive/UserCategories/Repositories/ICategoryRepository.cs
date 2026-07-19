using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.Database.Entities.UserInteractive.UserCategories.Repositories;

/// <summary>
/// Repository interface for working with user categories
/// </summary>
public interface ICategoryRepository
{
    /// <summary>
    /// Get all active categories
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active category DTOs</returns>
    Task<List<CategoryDto>> GetAllActiveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Get category by ID
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category DTO or null if not found</returns>
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Get category by name
    /// </summary>
    /// <param name="name">Category name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Category DTO or null if not found</returns>
    Task<CategoryDto?> GetByNameAsync(string name, CancellationToken cancellationToken);

    /// <summary>
    /// Get category by code
    /// </summary>
    /// <param name="code">Category code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<CategoryDto?> GetByCodeAsync(string code, CancellationToken cancellationToken);

    /// <summary>
    /// Create a new category
    /// </summary>
    /// <param name="dto">Category DTO</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created category DTO with generated ID</returns>
    Task<CategoryDto> CreateAsync(CategoryDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Update an existing category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="dto">Category DTO with update data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated category DTO or null if not found</returns>
    Task<CategoryDto?> UpdateAsync(int id, CategoryDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Delete a category
    /// </summary>
    /// <param name="id">Category ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if category was deleted</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Get categories for user
    /// </summary>
    /// <param name="externalUserId">External user id</param>
    /// <param name="expireDate">Date to filter expired categories</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of categories status DTOs</returns>
    Task<List<CategoryStatusDto>> GetCategoriesWithDuelStatusAsync(long externalUserId, DateTimeOffset expireDate, CancellationToken cancellationToken);
}