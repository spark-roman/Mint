namespace Mint.Database.Entities.UserInteractive.UserCategories.Dto;

/// <summary>
/// DTO for category
/// </summary>
public record CategoryDto
{
    /// <summary>
    /// Category ID
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Category name
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Category description
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Category code
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Whether AI will search news for this category
    /// </summary>
    public bool IsActiveForAI { get; init; }

    /// <summary>
    /// AI search keywords
    /// </summary>
    public string? SearchKeywords { get; init; }
}
