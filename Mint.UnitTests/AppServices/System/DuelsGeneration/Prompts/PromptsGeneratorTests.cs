using Mint.Database.Entities.UserInteractive.UserCategories.Dto;
using Mint.App.Services.System.DuelsGeneration.Prompts;
using Mint.Database.Entities.System.Prompts.Dto;

namespace Mint.UnitTests.AppServices.System.DuelsGeneration.Prompts;

/// <summary>
/// Tests for <see cref="PromptsGenerator"/>
/// </summary>
public class PromptsGeneratorTests
{
    private readonly PromptsGenerator _generator = new();

    private static AiPromptDto CreatePrompt(
        string systemPromptTemplate,
        string userPromptTemplate)
    {
        return new AiPromptDto
        {
            Id = 1,
            SystemPromptTemplate = systemPromptTemplate,
            UserPromptTemplate = userPromptTemplate,
            Temperature = 0.6f,
            MaxDuelsPerRun = 3,
            UpdatedAt = DateTimeOffset.UtcNow,
            Categories = []
        };
    }

    private static CategoryDto CreateCategory(
        string name,
        string code,
        string? description = null,
        string? searchKeywords = null)
    {
        return new CategoryDto
        {
            Id = 1,
            Name = name,
            Description = description,
            SearchKeywords = searchKeywords,
            Code = code
        };
    }

    #region GetSystemPrompt

    /// <summary>
    /// Verifies that null prompt throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void GetSystemPrompt_NullPrompt_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _generator.GetSystemPrompt(null!, default));
    }

    /// <summary>
    /// Verifies that system prompt returns the template unchanged.
    /// </summary>
    [Fact]
    public void GetSystemPrompt_ReturnsTemplateUnchanged()
    {
        // Arrange
        var expected = "You are a content generator.";
        var prompt = CreatePrompt(expected, "");

        // Act
        var result = _generator.GetSystemPrompt(prompt, default);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies that system prompt with newlines and special chars is preserved.
    /// </summary>
    [Fact]
    public void GetSystemPrompt_PreservesNewlinesAndSpecialChars()
    {
        // Arrange
        var template = "Line 1\nLine 2\nLine 3\n\n\nMultiple blank lines.";
        var prompt = CreatePrompt(template, "");

        // Act
        var result = _generator.GetSystemPrompt(prompt, default);

        // Assert
        Assert.Equal(template, result);
    }

    #endregion

    #region GetUserPrompt

    /// <summary>
    /// Verifies that null prompt throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void GetUserPrompt_NullPrompt_ThrowsArgumentNullException()
    {
        // Arrange
        var category = CreateCategory("tech", "tech");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _generator.GetUserPrompt(null!, category, 3, default));
    }

    /// <summary>
    /// Verifies that null category throws ArgumentNullException.
    /// </summary>
    [Fact]
    public void GetUserPrompt_NullCategory_ThrowsArgumentNullException()
    {
        // Arrange
        var prompt = CreatePrompt("", "");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _generator.GetUserPrompt(prompt, null!, 3, default));
    }

    /// <summary>
    /// Verifies that {{count}} is replaced with the provided count.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ReplacesCount()
    {
        // Arrange
        var prompt = CreatePrompt("", "Generate {{count}} duels.");
        var category = CreateCategory("tech", "tech");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 5, default);

        // Assert
        Assert.Contains("Generate 5 duels.", result);
    }

    /// <summary>
    /// Verifies that {{count}} is replaced with count 1.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ReplacesCountOne()
    {
        // Arrange
        var prompt = CreatePrompt("", "Generate {{count}} duel.");
        var category = CreateCategory("tech", "tech");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 1, default);

        // Assert
        Assert.Contains("Generate 1 duel.", result);
    }

    /// <summary>
    /// Verifies that {{category_name}} is replaced with category name.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ReplacesCategoryName()
    {
        // Arrange
        var prompt = CreatePrompt("", "Category: {{category_name}}");
        var category = CreateCategory("Neural Networks & AI", "tech");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.Contains("Category: Neural Networks & AI", result);
    }

    /// <summary>
    /// Verifies that {{category_description}} is replaced with category description.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ReplacesCategoryDescription()
    {
        // Arrange
        var prompt = CreatePrompt("", "Desc: {{category_description}}");
        var category = CreateCategory("tech", "tech", "Description for tech");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.Contains("Desc: Description for tech", result);
    }

    /// <summary>
    /// Verifies that {{category_description}} uses default when null.
    /// </summary>
    [Fact]
    public void GetUserPrompt_NullDescription_UsesDefault()
    {
        // Arrange
        var prompt = CreatePrompt("", "Desc: {{category_description}}");
        var category = CreateCategory("tech", "tech", description: null);

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.Contains("Desc: Без описания", result);
    }

    /// <summary>
    /// Verifies that {{search_keywords}} is replaced with keywords.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ReplacesSearchKeywords()
    {
        // Arrange
        var prompt = CreatePrompt("", "Keywords: {{search_keywords}}");
        var category = CreateCategory("tech", "tech", null, "AI, crypto");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.Contains("Keywords: AI, crypto", result);
    }

    /// <summary>
    /// Verifies that all placeholders are replaced in one call.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ReplacesAllPlaceholders()
    {
        // Arrange
        var prompt = CreatePrompt(
            "",
            "Count: {{count}}, Name: {{category_name}}, Desc: {{category_description}}, Keywords: {{search_keywords}}");
        var category = CreateCategory(
            "Tech",
            "tech",
            "Technology desc",
            "AI, crypto");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 5, default);

        // Assert
        Assert.Contains("Count: 5", result);
        Assert.Contains("Name: Tech", result);
        Assert.Contains("Desc: Technology desc", result);
        Assert.Contains("Keywords: AI, crypto", result);
    }

    #endregion

    #region Conditional Blocks

    /// <summary>
    /// Verifies that {{news_date}} is replaced in system prompt.
    /// </summary>
    [Fact]
    public void GetSystemPrompt_ReplacesNewsDate()
    {
        // Arrange
        var template = "News from {{news_date}}";
        var prompt = CreatePrompt(template, "");
        var newsDate = new DateTimeOffset(2026, 11, 4, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = _generator.GetSystemPrompt(prompt, newsDate);

        // Assert
        Assert.Contains("News from 04.11.2026", result);
    }

    /// <summary>
    /// Verifies that {{news_date}} is replaced in user prompt.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ReplacesNewsDate()
    {
        // Arrange
        var prompt = CreatePrompt("", "Today is {{news_date}}");
        var category = CreateCategory("tech", "tech");
        var newsDate = new DateTimeOffset(2026, 11, 4, 0, 0, 0, TimeSpan.Zero);

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, newsDate);

        // Assert
        Assert.Contains("Today is 04.11.2026", result);
    }

    /// <summary>
    /// Verifies that {{#if}} block is rendered when search_keywords has value.
    /// </summary>
    [Fact]
    public void GetUserPrompt_IfBlockWithSearchKeywords_Rendered()
    {
        // Arrange
        var template = "Start\n{{#if search_keywords}}Keywords: {{search_keywords}}{{/if}}\nEnd";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech", null, "AI, crypto");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.Contains("Keywords: AI, crypto", result);
    }

    /// <summary>
    /// Verifies that {{#if}} block is removed when search_keywords is empty.
    /// </summary>
    [Fact]
    public void GetUserPrompt_IfBlockWithoutSearchKeywords_Removed()
    {
        // Arrange
        var template = "Start\n{{#if search_keywords}}Keywords: {{search_keywords}}\n{{/if}}\nEnd";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech", null, null);

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.DoesNotContain("Keywords:", result);
        Assert.DoesNotContain("{{#if", result);
        Assert.DoesNotContain("{{/if}}", result);
    }

    /// <summary>
    /// Verifies that {{#if}} block is rendered when description has value.
    /// </summary>
    [Fact]
    public void GetUserPrompt_IfBlockWithDescription_Rendered()
    {
        // Arrange
        var template = "Start\n{{#if description}}{{category_description}}{{/if}}\nEnd";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech", "Some description", null);

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.Contains("Some description", result);
    }

    /// <summary>
    /// Verifies that {{#if}} block is removed when description is null.
    /// </summary>
    [Fact]
    public void GetUserPrompt_IfBlockWithoutDescription_Removed()
    {
        // Arrange
        var template = "Start\n{{#if description}}Description here{{/if}}\nEnd";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech", null, null);

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.DoesNotContain("Description here", result);
        Assert.DoesNotContain("{{#if", result);
        Assert.DoesNotContain("{{/if}}", result);
    }

    /// <summary>
    /// Verifies that multiple {{#if}} blocks are processed correctly.
    /// </summary>
    [Fact]
    public void GetUserPrompt_MultipleIfBlocks_ProcessedCorrectly()
    {
        // Arrange
        var template = "{{#if search_keywords}}KW: {{search_keywords}}{{/if}}\n{{#if description}}DESC: {{category_description}}{{/if}}";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech", "Tech desc", "AI");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.Contains("KW: AI", result);
        Assert.Contains("DESC: Tech desc", result);
        Assert.DoesNotContain("{{#if", result);
    }

    /// <summary>
    /// Verifies that {{#if}} blocks are both removed when both are empty.
    /// </summary>
    [Fact]
    public void GetUserPrompt_MultipleIfBlocksBothEmpty_AllRemoved()
    {
        // Arrange
        var template = "{{#if search_keywords}}KW: {{search_keywords}}{{/if}}\n{{#if description}}DESC: {{category_description}}{{/if}}";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech", null, null);

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.DoesNotContain("KW:", result);
        Assert.DoesNotContain("DESC:", result);
        Assert.DoesNotContain("{{#if", result);
    }

    #endregion

    #region Whitespace Normalization

    /// <summary>
    /// Verifies that multiple consecutive blank lines are collapsed to one.
    /// </summary>
    [Fact]
    public void GetUserPrompt_MultipleBlankLines_CollapsedToOne()
    {
        // Arrange
        var template = "Line 1\n\n\n\nLine 2";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.DoesNotContain("\n\n\n", result);
    }

    /// <summary>
    /// Verifies that double blank lines are also collapsed (regex \n\s*\n).
    /// </summary>
    [Fact]
    public void GetUserPrompt_DoubleBlankLines_CollapsedToOne()
    {
        // Arrange
        var template = "Line 1\n\nLine 2";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert
        Assert.DoesNotContain("\n\n", result);
    }

    #endregion

    #region Else Not Supported

    /// <summary>
    /// Verifies that {{else}} is NOT processed and remains as-is.
    /// The validator only supports {{#if}}...{{/if}} without else.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ElseBlock_NotProcessed_RemainsAsIs()
    {
        // Arrange
        var template = "{{#if search_keywords}}KW: {{search_keywords}}{{/if}}{{else}}Other{{/if}}";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech", null, null);

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert - else block remains untouched since it's not supported
        Assert.DoesNotContain("KW:", result);
        Assert.Contains("{{else}}", result);
        Assert.Contains("Other", result);
    }

    /// <summary>
    /// Verifies that {{else}} inside if block is preserved when condition is true.
    /// The regex matches to the first {{/if}}, so {{else}} remains in the content.
    /// </summary>
    [Fact]
    public void GetUserPrompt_ElseInsideIfBlock_Preserved()
    {
        // Arrange
        var template = "{{#if search_keywords}}Use: {{search_keywords}}{{else}}Fallback{{/if}}";
        var prompt = CreatePrompt("", template);
        var category = CreateCategory("tech", "tech", null, "AI");

        // Act
        var result = _generator.GetUserPrompt(prompt, category, 3, default);

        // Assert - the whole block is replaced with content ({{else}} is part of content), {{/if}} consumed
        Assert.Contains("Use: AI{{else}}Fallback", result);
    }

    #endregion
}
