namespace Mint.UnitTests.AppServices.System.Bot.Messages;

/// <summary>
/// Tests for <see cref="Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper"/>.
/// </summary>
public class MarkdownV2EscaperTests
{
    /// <summary>
    /// Verifies that Escape returns null when the input is null.
    /// </summary>
    [Fact]
    public void Escape_NullInput_ReturnsNull()
    {
        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape(null!);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Verifies that Escape returns empty string when the input is empty.
    /// </summary>
    [Fact]
    public void Escape_EmptyInput_ReturnsEmpty()
    {
        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape(string.Empty);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    /// <summary>
    /// Verifies that Escape returns the same text when there are no special characters.
    /// </summary>
    [Fact]
    public void Escape_TextWithoutSpecialChars_ReturnsUnchanged()
    {
        // Arrange
        var text = "Просто текст 123";

        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape(text);

        // Assert
        Assert.Equal(text, result);
    }

    /// <summary>
    /// Verifies that Escape escapes all MarkdownV2 special characters.
    /// </summary>
    [Fact]
    public void Escape_AllSpecialChars_EscapesEachChar()
    {
        // Arrange
        var text = "_*[]()~`>#+-=|{}.!";

        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape(text);

        // Assert
        Assert.Equal("\\_\\*\\[\\]\\(\\)\\~\\`\\>\\#\\+\\-\\=\\|\\{\\}\\.\\!", result);
    }

    /// <summary>
    /// Verifies that Escape escapes a single special character.
    /// </summary>
    [Theory]
    [InlineData('_', "\\_")]
    [InlineData('*', "\\*")]
    [InlineData('[', "\\[")]
    [InlineData(']', "\\]")]
    [InlineData('(', "\\(")]
    [InlineData(')', "\\)")]
    [InlineData('~', "\\~")]
    [InlineData('`', "\\`")]
    [InlineData('>', "\\>")]
    [InlineData('#', "\\#")]
    [InlineData('+', "\\+")]
    [InlineData('-', "\\-")]
    [InlineData('=', "\\=")]
    [InlineData('|', "\\|")]
    [InlineData('{', "\\{")]
    [InlineData('}', "\\}")]
    [InlineData('.', "\\.")]
    [InlineData('!', "\\!")]
    public void Escape_SingleSpecialChar_EscapesIt(char specialChar, string expected)
    {
        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape(specialChar.ToString());

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// Verifies that Escape does not escape regular characters.
    /// </summary>
    [Theory]
    [InlineData('a')]
    [InlineData('Z')]
    [InlineData('0')]
    [InlineData('9')]
    [InlineData('й')]
    [InlineData('Я')]
    [InlineData(' ')]
    [InlineData(':')]
    public void Escape_RegularChar_DoesNotEscape(char regularChar)
    {
        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape(regularChar.ToString());

        // Assert
        Assert.Equal(regularChar.ToString(), result);
    }

    /// <summary>
    /// Verifies that Escape does not escape multi-byte characters like emoji.
    /// </summary>
    [Fact]
    public void Escape_Emoji_DoesNotEscape()
    {
        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape("🥇");

        // Assert
        Assert.Equal("🥇", result);
    }

    /// <summary>
    /// Verifies that Escape escapes special characters in the middle of text (user name case).
    /// </summary>
    [Fact]
    public void Escape_UserNameWithSpecialChars_EscapesOnlySpecialChars()
    {
        // Arrange
        var text = "user_name.2026";

        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape(text);

        // Assert
        Assert.Equal("user\\_name\\.2026", result);
    }

    /// <summary>
    /// Verifies that Escape handles a text with mixed special and regular characters.
    /// </summary>
    [Fact]
    public void Escape_MixedText_EscapesOnlySpecialChars()
    {
        // Arrange
        var text = "Текст (в скобках) и *жирный*!";

        // Act
        var result = Mint.App.Services.System.Bot.Handlers.Messages.MarkdownV2Escaper.Escape(text);

        // Assert
        Assert.Equal("Текст \\(в скобках\\) и \\*жирный\\*\\!", result);
    }
}