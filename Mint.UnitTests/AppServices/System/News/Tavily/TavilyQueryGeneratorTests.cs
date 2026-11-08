using FluentAssertions;
using Mint.App.Services.System.News.Tavily;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;

namespace Mint.UnitTests.AppServices.System.News.Tavily;

/// <summary>
/// Tests for <see cref="TavilyQueryGenerator"/>
/// </summary>
public class TavilyQueryGeneratorTests
{
    [Fact]
    public void GenerateQuery_ValidCategory_ReturnsJoinedKeywords()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Test Category",
            Code = "test",
            SearchKeywords = "keyword1, keyword2, keyword3",
            IsActiveForAI = true
        };

        // Act
        var result = TavilyQueryGenerator.GenerateQuery(category);

        // Assert
        result.Should().Be("keyword1 keyword2 keyword3");
    }

    [Fact]
    public void GenerateQuery_SingleKeyword_ReturnsKeyword()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Test",
            Code = "test",
            SearchKeywords = "bitcoin",
            IsActiveForAI = true
        };

        // Act
        var result = TavilyQueryGenerator.GenerateQuery(category);

        // Assert
        result.Should().Be("bitcoin");
    }

    [Fact]
    public void GenerateQuery_MultiWordKeywords_PreservesSpaces()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Test",
            Code = "test",
            SearchKeywords = "биткоин курс, Ethereum ETF, Bitcoin ATM",
            IsActiveForAI = true
        };

        // Act
        var result = TavilyQueryGenerator.GenerateQuery(category);

        // Assert
        result.Should().Be("биткоин курс Ethereum ETF Bitcoin ATM");
    }

    [Fact]
    public void GenerateQuery_EmptyKeywords_ThrowsInvalidOperationException()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Test",
            Code = "test",
            SearchKeywords = "",
            IsActiveForAI = true
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => TavilyQueryGenerator.GenerateQuery(category));
    }

    [Fact]
    public void GenerateQuery_NullKeywords_ThrowsInvalidOperationException()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Test",
            Code = "test",
            SearchKeywords = null,
            IsActiveForAI = true
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => TavilyQueryGenerator.GenerateQuery(category));
    }

    [Fact]
    public void GenerateQuery_NullCategory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => TavilyQueryGenerator.GenerateQuery(null!));
    }

    [Fact]
    public void GenerateQuery_WithTrailingCommas_IgnoresEmptySegments()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Test",
            Code = "test",
            SearchKeywords = "ai, chatgpt, ",
            IsActiveForAI = true
        };

        // Act
        var result = TavilyQueryGenerator.GenerateQuery(category);

        // Assert
        result.Should().Be("ai chatgpt");
    }

    [Fact]
    public void GenerateQuery_WithMultipleSpaces_Results_InSingleSpace()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Test",
            Code = "test",
            SearchKeywords = "ai,   chatgpt,   gpt",
            IsActiveForAI = true
        };

        // Act
        var result = TavilyQueryGenerator.GenerateQuery(category);

        // Assert
        result.Should().Be("ai chatgpt gpt");
    }

    [Fact]
    public void GenerateQuery_RussianAndEnglishKeywords_WorksCorrectly()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Тест",
            Code = "ai",
            SearchKeywords = "OpenAI ChatGPT Claude Gemini xAI ИИ нейросети",
            IsActiveForAI = true
        };

        // Act
        var result = TavilyQueryGenerator.GenerateQuery(category);

        // Assert
        result.Should().Be("OpenAI ChatGPT Claude Gemini xAI ИИ нейросети");
    }

    [Fact]
    public void GenerateQuery_NoCommaOnlyText_ReturnsAsIs()
    {
        // Arrange
        var category = new CategoryDto
        {
            Id = 1,
            Name = "Test",
            Code = "test",
            SearchKeywords = "space NASA SpaceX discovery",
            IsActiveForAI = true
        };

        // Act
        var result = TavilyQueryGenerator.GenerateQuery(category);

        // Assert
        result.Should().Be("space NASA SpaceX discovery");
    }
}
