using System.Globalization;
using System.Text.RegularExpressions;
using Mint.Database.Entities.System.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Prompts;

/// <inheritdoc/>
public partial class PromptsGenerator : IPromptsGenerator
{
    /// <inheritdoc/>
    public string GetSystemPrompt(AiPromptDto prompt)
    {
        ArgumentNullException.ThrowIfNull(prompt);

        return prompt.SystemPromptTemplate;
    }

    /// <inheritdoc/>
    public string GetUserPrompt(AiPromptDto prompt, CategoryDto category, int count)
    {
        ArgumentNullException.ThrowIfNull(prompt);
        ArgumentNullException.ThrowIfNull(category);

        var template = prompt.UserPromptTemplate;
        
        var result = template
            .Replace("{{count}}", count.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCultureIgnoreCase)
            .Replace("{{category_name}}", category.Name, StringComparison.InvariantCultureIgnoreCase)
            .Replace("{{category_description}}", category.Description ?? "Без описания", StringComparison.InvariantCultureIgnoreCase)
            .Replace("{{search_keywords}}", category.SearchKeywords ?? "", StringComparison.InvariantCultureIgnoreCase);

        result = ProcessConditionalBlocks(result, category);

        return result;
    }

    private static string ProcessConditionalBlocks(string template, CategoryDto category)
    {
        var ifPattern = @"\{\{#if (\w+)\}\}(.*?)\{\{\/if\}\}";
        var matches = Regex.Matches(template, ifPattern, RegexOptions.Singleline);

        foreach (Match match in matches)
        {
            var variableName = match.Groups[1].Value;
            var content = match.Groups[2].Value;
            
            var hasValue = variableName switch
            {
                "search_keywords" => !string.IsNullOrEmpty(category.SearchKeywords),
                "description" => !string.IsNullOrEmpty(category.Description),
                _ => false
            };

            var replacement = hasValue ? content : string.Empty;
            template = template.Replace(match.Value, replacement, StringComparison.InvariantCultureIgnoreCase);
        }

        template = MyRegex().Replace(template, "\n");

        return template;
    }

    [GeneratedRegex(@"\n\s*\n")]
    private static partial Regex MyRegex();
}
