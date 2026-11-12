using System.Collections.ObjectModel;
using Mint.Database.Entities.News.Dto;
using Mint.Database.Entities.System.Prompts.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Prompts;

/// <summary>
/// Prompt generator interface
/// </summary>
public interface IPromptsGenerator
{
    /// <summary>
    /// Get system prompt for AI generation
    /// </summary>
    string GetSystemPrompt(AiPromptDto prompt, DateTimeOffset newsDate);

    /// <summary>
    /// Get user prompt for AI generation
    /// </summary>
    string GetUserPrompt(AiPromptDto prompt, CategoryDto category, int count, DateTimeOffset newsDate);

    /// <summary>
    /// Get user prompt for duel generation from news
    /// </summary>
    string GetUserPromptFromNews(AiPromptDto prompt, Collection<NewsDto> news, int count, string? categoryName = null, string? categoryCode = null, string? categoryDescription = null);
}
