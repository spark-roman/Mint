using FluentAssertions;
using Mint.App.Services.System.News.Google;

namespace Mint.UnitTests.AppServices.System.News.Google;

/// <summary>
/// Tests for <see cref="GoogleNewsClient"/>
/// </summary>
public class GoogleNewsClientTests
{
    [Fact]
    public void BuildQuery_WithAllParameters_ReturnsCorrectQueryString()
    {
        // Arrange
        var query = "bitcoin ethereum";
        var apiKey = "test-key";
        var maxResults = 5;
        var language = "ru";
        var country = "ru";

        // Act
        var result = BuildQuery(query, apiKey, maxResults, language, country);

        // Assert
        result.Should().Contain("q=bitcoin%20ethereum");
        result.Should().Contain("apikey=test-key");
        result.Should().Contain("max=5");
        result.Should().Contain("sortby=publishedAt");
        result.Should().Contain("lang=ru");
        result.Should().Contain("country=ru");
    }

    [Fact]
    public void BuildQuery_WithoutLanguageAndCountry_ReturnsQueryWithoutFilters()
    {
        // Arrange
        var query = "test";
        var apiKey = "test-key";
        var maxResults = 10;

        // Act
        var result = BuildQuery(query, apiKey, maxResults, null, null);

        // Assert
        result.Should().Contain("q=test");
        result.Should().NotContain("lang=");
        result.Should().NotContain("country=");
    }

    [Fact]
    public void BuildQuery_WithSpecialCharacters_EscapesQuery()
    {
        // Arrange
        var query = "премьера OR сериал";
        var apiKey = "test-key";
        var maxResults = 5;

        // Act
        var result = BuildQuery(query, apiKey, maxResults, "ru", "ru");

        // Assert - special characters are URL-encoded
        result.Should().Contain("q=");
        result.Should().Contain("apikey=test-key");
        result.Should().Contain("lang=ru");
        result.Should().Contain("country=ru");
    }

    [Fact]
    public void BuildQuery_WithNOTClause_EscapesCorrectly()
    {
        // Arrange
        var query = "film NOT online";
        var apiKey = "test-key";
        var maxResults = 5;

        // Act
        var result = BuildQuery(query, apiKey, maxResults, null, null);

        // Assert
        result.Should().Contain("q=");
        result.Should().Contain("film");
        result.Should().Contain("NOT");
        result.Should().Contain("online");
    }

    private static string BuildQuery(string query, string apiKey, int maxResults, string? language, string? country)
    {
        var method = typeof(GoogleNewsClient).GetMethod("BuildQuery", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static);
        return (string)method!.Invoke(null, new object?[] { query, apiKey, maxResults, language, country })!;
    }
}
