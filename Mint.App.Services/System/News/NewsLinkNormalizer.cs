namespace Mint.App.Services.System.News;

/// <summary>
/// Utility methods for news link normalization and deduplication.
/// </summary>
public static class NewsLinkNormalizer
{
    /// <summary>
    /// Normalize a URL link for deduplication — uppercase host, no www, no trailing slash.
    /// </summary>
    public static string Normalize(string link)
    {
        var uri = new Uri(link, UriKind.Absolute);
        var host = uri.Host.ToUpperInvariant();
        if (host.StartsWith("WWW.", StringComparison.Ordinal))
            host = host.Substring(4);

        var path = uri.AbsolutePath;
        if (path.Length > 1 && path.EndsWith('/'))
            path = path.TrimEnd('/');

        return $"{uri.Scheme}://{host}{path}";
    }
}
