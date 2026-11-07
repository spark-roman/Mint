using System.Globalization;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.News.Tavily;

/// <summary>
/// Generates Tavily search queries based on category configuration
/// </summary>
public static class TavilyQueryGenerator
{
    /// <summary>
    /// Category query templates mapping
    /// </summary>
    private static readonly Dictionary<string, (string Query, string Language)> _templates = new()
    {
        ["ai"] = ("AI and neural networks breaking news tech crunch {targetDate}", "🇬🇧 Eng"),
        ["hardware"] = ("latest tech gadgets hardware release Nvidia AMD Apple {targetDate}", "🇬🇧 Eng"),
        ["crypto"] = ("crypto breaking news bitcoin ethereum market crash hack {targetDate}", "🇬🇧 Eng"),
        ["games"] = ("gaming news twitch streamer scandal game release {targetDate}", "🇬🇧 Eng"),
        ["esports"] = ("esports tournament cs2 dota2 match controversy {targetDate}", "🇬🇧 Eng"),
        ["science"] = ("space discovery NASA astronomy science breakthrough {targetDate}", "🇬🇧 Eng"),
        ["movies"] = ("новости кино премьеры сериалов скандалы актеры за {targetDate}", "🇷🇺 Rus"),
        ["memes"] = ("главные мемы тренды обсуждения в рунете за {targetDate}", "🇷🇺 Rus"),
        ["cars"] = ("новости авторынка электромобили zeekr xiaomi лада за {targetDate}", "🇷🇺 Rus"),
        ["soccer"] = ("главные спортивные новости футбол рпл лига чемпионов {targetDate}", "🇷🇺 Rus"),
        ["fashion"] = ("pop music billboard fashion week celebrity scandal {targetDate}", "🇬🇧/🇷🇺"),
        ["finance"] = ("новости финтех банки маркетплейсы wildberries ozon за {targetDate}", "🇷🇺 Rus")
    };

    /// <summary>
    /// Generate Tavily search query for a category
    /// </summary>
    /// <param name="category">Category to generate query for</param>
    /// <param name="targetDate">Target date for the query</param>
    /// <returns>Tavily search query</returns>
    public static string GenerateQuery(CategoryDto category, DateTimeOffset targetDate)
    {
        ArgumentNullException.ThrowIfNull(category);

        var template = _templates.GetValueOrDefault(category.Code);

        if (string.IsNullOrEmpty(template.Query))
        {
            throw new InvalidOperationException($"No Tavily query template found for category '{category.Code}'");
        }

        return template.Query.Replace("{targetDate}", targetDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), StringComparison.Ordinal);
    }

    /// <summary>
    /// Get all available category codes
    /// </summary>
    public static ICollection<string> AvailableCategories => _templates.Keys;
}
