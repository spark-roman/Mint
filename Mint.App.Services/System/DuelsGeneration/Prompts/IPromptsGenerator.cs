using Mint.Database.Entities.System.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Prompts;

/// <summary>
/// Generates prompts for a given category.
/// </summary>
public interface IPromptsGenerator
{
    /// <summary>
    /// Generate final system prompt
    /// </summary>
    /// <param name="prompt">Prompt with template</param>
    /// <returns>Final system prompt for AI</returns>
    string GetSystemPrompt(AiPromptDto prompt);

    /// <summary>
    /// Generate final user prompt
    /// </summary>
    /// <param name="prompt">Prompt with template</param>
    /// <param name="category">Category of the prompt</param>
    /// <param name="count">Count of generated results</param>
    /// <returns>Final user prompt</returns> 
    string GetUserPrompt(AiPromptDto prompt, CategoryDto category, int count);
}
