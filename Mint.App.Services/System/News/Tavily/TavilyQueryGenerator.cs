using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.News.Tavily;

/// <summary>
/// Generates Tavily search queries from category SearchKeywords.
/// </summary>
public static class TavilyQueryGenerator
{
    /// <summary>
    /// Generate Tavily search query from category SearchKeywords.
    /// Keywords are split by comma, trimmed, and joined by space.
    /// </summary>
    /// <param name="category">Category with SearchKeywords</param>
    /// <returns>Tavily search query</returns>
    public static string GenerateQuery(CategoryDto category)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (string.IsNullOrWhiteSpace(category.SearchKeywords))
        {
            throw new InvalidOperationException($"No search keywords found for category '{category.Code}'");
        }

        // Split by comma, trim, filter empty, join by space
        var keywords = category.SearchKeywords
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(k => !string.IsNullOrWhiteSpace(k));

        return string.Join(" ", keywords);
    }
}
