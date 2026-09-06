namespace Mint.App.Services.System.Bot.Handlers.Messages;

/// <summary>
/// Escapes markdown v2
/// </summary>
public static class MarkdownV2Escaper
{
    /// <summary>
    /// Escapes markdown v2
    /// </summary>
    /// <param name="text">Text to escape</param>
    /// <returns>Escaped text</returns>
    public static string Escape(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        ReadOnlySpan<char> source = text.AsSpan();
        
        int specialCharsCount = 0;
        foreach (char ch in source)
        {
            if (IsSpecialChar(ch)) specialCharsCount++;
        }

        if (specialCharsCount == 0) return text;

        int targetLength = source.Length + specialCharsCount;

        return string.Create(targetLength, text, (targetSpan, sourceText) =>
        {
            ReadOnlySpan<char> srcSpan = sourceText.AsSpan();
            int targetIdx = 0;

            foreach (char ch in srcSpan)
            {
                if (IsSpecialChar(ch))
                {
                    targetSpan[targetIdx++] = '\\';
                }
                targetSpan[targetIdx++] = ch;
            }
        });
    }

    private static bool IsSpecialChar(char ch)
    {
        return ch switch
        {
            '_' or '*' or '[' or ']' or '(' or ')' or '~' or '`' or '>' or 
            '#' or '+' or '-' or '=' or '|' or '{' or '}' or '.' or '!' => true,
            _ => false
        };
    }
}
