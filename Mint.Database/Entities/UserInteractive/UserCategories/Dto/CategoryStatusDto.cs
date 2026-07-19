namespace Mint.Database.Entities.UserInteractive.UserCategories.Dto;

/// <summary>
/// Status of a category
/// </summary>
public record CategoryStatusDto
{
    /// <summary>
    /// Category Id
    /// </summary>
    public required long CategoryId { get; init; }

    /// <summary>
    /// Category name
    /// </summary>
    public required string CategoryName { get; init; }

    /// <summary>
    /// Category code
    /// </summary>
    public required string CategoryCode { get; init; }

    /// <summary>
    /// Category emoji
    /// </summary>
    public required string CategoryEmoji { get; init; }

    /// <summary>
    /// Category status
    /// </summary>
    public required bool IsActive { get; init; }

    /// <summary>
    /// Category status emoji
    /// </summary>
    public string? StatusEmoji => HasAvailableDuels ? "🟢" : "🔴";

    /// <summary>
    /// Category has available duels
    /// </summary>
    public bool HasAvailableDuels { get; init; }
}
