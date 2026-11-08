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
        // Arrange
        var client = CreateClient();

        // Act
        var result = InvokeParseDate(client, "Fri, 06 Nov 2026 18:00:00 GMT");

        // Assert
        result.Should().BeCloseTo(new DateTimeOffset(2026, 11, 6, 18, 0, 0, TimeSpan.Zero), TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void ParseDate_EmptyString_ReturnsFallback()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var result = InvokeParseDate(client, "");

        // Assert
        result.Should().Be(_fixedNow);
    }

    [Fact]
    public void ParseDate_NullString_ReturnsFallback()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var result = InvokeParseDate(client, null!);

        // Assert
        result.Should().Be(_fixedNow);
    }

    [Fact]
    public void ParseDate_InvalidFormat_ReturnsFallback()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var result = InvokeParseDate(client, "not-a-date");

        // Assert
        result.Should().Be(_fixedNow);
    }

    [Fact]
    public void ParseDate_WithTimezone_ReturnsCorrectTime()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var result = InvokeParseDate(client, "Thu, 05 Nov 2026 10:30:00 GMT");

        // Assert
        result.Should().BeCloseTo(new DateTimeOffset(2026, 11, 5, 10, 30, 0, TimeSpan.Zero), TimeSpan.FromSeconds(1));
    }

    #endregion

    #region CalculateTimeRange

    [Fact]
    public void CalculateTimeRange_DaysBack1_ReturnsDay()
    {
        // Arrange
        _mockSettings.Setup(s => s.Value.DaysBack).Returns(1);
        var client = CreateClient();

        // Act
        var result = InvokeCalculateTimeRange(client);

        // Assert
        result.Should().Be("day");
    }

    [Fact]
    public void CalculateTimeRange_DaysBack7_ReturnsWeek()
    {
        // Arrange
        _mockSettings.Setup(s => s.Value.DaysBack).Returns(7);
        var client = CreateClient();

        // Act
        var result = InvokeCalculateTimeRange(client);

        // Assert
        result.Should().Be("week");
    }

    [Fact]
    public void CalculateTimeRange_DaysBack30_ReturnsMonth()
    {
        // Arrange
        _mockSettings.Setup(s => s.Value.DaysBack).Returns(30);
        var client = CreateClient();

        // Act
        var result = InvokeCalculateTimeRange(client);

        // Assert
        result.Should().Be("month");
    }

    [Fact]
    public void CalculateTimeRange_DaysBack365_ReturnsYear()
    {
        // Arrange
        _mockSettings.Setup(s => s.Value.DaysBack).Returns(365);
        var client = CreateClient();

        // Act
        var result = InvokeCalculateTimeRange(client);

        // Assert
        result.Should().Be("year");
    }

    [Fact]
    public void CalculateTimeRange_DaysBackOver365_ReturnsYear()
    {
        // Arrange
        _mockSettings.Setup(s => s.Value.DaysBack).Returns(400);
        var client = CreateClient();

        // Act
        var result = InvokeCalculateTimeRange(client);

        // Assert
        result.Should().Be("year");
    }

    [Fact]
    public void CalculateTimeRange_DaysBack0_ReturnsDay()
    {
        // Arrange
        _mockSettings.Setup(s => s.Value.DaysBack).Returns(0);
        var client = CreateClient();

        // Act
        var result = InvokeCalculateTimeRange(client);

        // Assert
        result.Should().Be("day");
    }

    #endregion

    #region Helper Methods

    private TavilyClient CreateClient()
    {
        return new TavilyClient(_mockSettings.Object, null!, _timeProvider, _mockLogger.Object);
    }

    private DateTimeOffset InvokeParseDate(TavilyClient client, string? raw)
    {
        var method = typeof(TavilyClient).GetMethod("ParseDate", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static);
        return (DateTimeOffset)method!.Invoke(client, new object?[] { raw, _fixedNow })!;
    }

    private string InvokeCalculateTimeRange(TavilyClient client)
    {
        var method = typeof(TavilyClient).GetMethod("CalculateTimeRange", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static);
        return (string)method!.Invoke(client, new object?[] { _mockSettings.Object.Value.DaysBack })!;
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
