using System.Collections.ObjectModel;
using FluentAssertions;
using Gnews;
using Mint.App.Services.System.News.Google;

namespace Mint.UnitTests.AppServices.System.News.Google;

/// <summary>
/// Tests for <see cref="GoogleNewsClient"/>
/// </summary>
public class GoogleNewsClientTests
{
    #region ParseLang

    [Fact]
    public void ParseLang_ValidLanguage_ReturnsEnum()
    {
        // Act
        var result = InvokeParseLang("Ru");

        // Assert
        result.Should().Be(Gnews.Constants.Lang.Ru);
    }

    [Fact]
    public void ParseLang_InvalidLanguage_ReturnsNull()
    {
        // Act
        var result = InvokeParseLang("invalid");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseLang_NullLanguage_ReturnsNull()
    {
        // Act
        var result = InvokeParseLang(null!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseLang_EmptyLanguage_ReturnsNull()
    {
        // Act
        var result = InvokeParseLang("");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseLang_CaseInsensitive_ReturnsEnum()
    {
        // Act
        var result = InvokeParseLang("ru");

        // Assert
        result.Should().Be(Gnews.Constants.Lang.Ru);
    }

    #endregion

    #region ParseCountry

    [Fact]
    public void ParseCountry_ValidCountry_ReturnsEnum()
    {
        // Act
        var result = InvokeParseCountry("Ru");

        // Assert
        result.Should().Be(Gnews.Constants.Country.Ru);
    }

    [Fact]
    public void ParseCountry_InvalidCountry_ReturnsNull()
    {
        // Act
        var result = InvokeParseCountry("invalid");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseCountry_NullCountry_ReturnsNull()
    {
        // Act
        var result = InvokeParseCountry(null!);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseCountry_EmptyCountry_ReturnsNull()
    {
        // Act
        var result = InvokeParseCountry("");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ParseCountry_CaseInsensitive_ReturnsEnum()
    {
        // Act
        var result = InvokeParseCountry("us");

        // Assert
        result.Should().Be(Gnews.Constants.Country.Us);
    }

    #endregion

    #region Helper Methods

    private Gnews.Constants.Lang? InvokeParseLang(string? lang)
    {
        var method = typeof(GoogleNewsClient).GetMethod("ParseLang", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static);
        return (Gnews.Constants.Lang?)method!.Invoke(null, new object?[] { lang })!;
    }

    private Gnews.Constants.Country? InvokeParseCountry(string? country)
    {
        var method = typeof(GoogleNewsClient).GetMethod("ParseCountry", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Static);
        return (Gnews.Constants.Country?)method!.Invoke(null, new object?[] { country })!;
    }

    #endregion
}
