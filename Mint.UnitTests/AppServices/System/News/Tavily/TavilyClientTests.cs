using System.Collections.ObjectModel;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.App.Services.System.News.Tavily;
using Mint.Database.Entities.News.Dto;
using Moq;

namespace Mint.UnitTests.AppServices.System.News.Tavily;

/// <summary>
/// Tests for <see cref="TavilyClient"/>
/// </summary>
public class TavilyClientTests
{
    private readonly Mock<IOptions<TavilySettings>> _mockSettings;
    private readonly Mock<ILogger<TavilyClient>> _mockLogger;
    private readonly TimeProvider _timeProvider;

    private readonly DateTimeOffset _fixedNow = new(2026, 11, 7, 12, 0, 0, TimeSpan.Zero);

    public TavilyClientTests()
    {
        _mockSettings = new Mock<IOptions<TavilySettings>>();
        _mockSettings.Setup(s => s.Value).Returns(new TavilySettings
        {
            ApiKey = "test-key",
            ApiUrl = new Uri("https://api.tavily.com/search"),
            MaxResults = 5,
            SearchDepth = "advanced",
            Topic = "news",
            DaysBack = 7,
            ExcludeDomains = new Collection<string> { "example.com" }
        });

        _mockLogger = new Mock<ILogger<TavilyClient>>();
        _timeProvider = new FakeTimeProvider(_fixedNow);
    }

    #region ParseDate

    [Fact]
    public void ParseDate_RFC2822Format_ReturnsParsedDate()
    {
        // Act
        var result = InvokeParseDate("Fri, 06 Nov 2026 18:00:00 GMT");

        // Assert
        result.Should().BeCloseTo(new DateTimeOffset(2026, 11, 6, 18, 0, 0, TimeSpan.Zero), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ParseDate_EmptyString_ReturnsFallback()
    {
        // Act
        var result = InvokeParseDate("");

        // Assert
        result.Should().Be(_fixedNow);
    }

    [Fact]
    public void ParseDate_NullString_ReturnsFallback()
    {
        // Act
        var result = InvokeParseDate(null!);

        // Assert
        result.Should().Be(_fixedNow);
    }

    [Fact]
    public void ParseDate_InvalidFormat_ReturnsFallback()
    {
        // Act
        var result = InvokeParseDate("not-a-date");

        // Assert
        result.Should().Be(_fixedNow);
    }

    [Fact]
    public void ParseDate_WithTimezone_ReturnsCorrectTime()
    {
        // Act
        var result = InvokeParseDate("Thu, 05 Nov 2026 10:30:00 GMT");

        // Assert
        result.Should().BeCloseTo(new DateTimeOffset(2026, 11, 5, 10, 30, 0, TimeSpan.Zero), TimeSpan.FromSeconds(1));
    }

    #endregion

    #region Helper Methods

    private TavilyClient CreateClient()
    {
        return new TavilyClient(_mockSettings.Object, null!, _timeProvider, _mockLogger.Object);
    }

    private DateTimeOffset InvokeParseDate(string? raw)
    {
        var method = typeof(TavilyClient).GetMethod("ParseDate", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static);
        return (DateTimeOffset)method!.Invoke(null, new object?[] { raw, _fixedNow })!;
    }

    #endregion
}

/// <summary>
/// Fake TimeProvider for deterministic testing.
/// </summary>
public sealed class FakeTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _fixedTime;

    public FakeTimeProvider(DateTimeOffset fixedTime)
    {
        _fixedTime = fixedTime;
    }

    public override DateTimeOffset GetUtcNow() => _fixedTime;
}
