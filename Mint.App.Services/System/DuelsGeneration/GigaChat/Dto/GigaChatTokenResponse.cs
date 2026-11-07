using System.Text.Json.Serialization;

namespace Mint.App.Services.System.DuelsGeneration.GigaChat.Dto;

/// <summary>
/// OAuth token response from GigaChat
/// </summary>
public record GigaChatTokenResponse
{
    /// <summary>
    /// Access token
    /// </summary>
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    /// <summary>
    /// Token lifetime in seconds
    /// </summary>
    [JsonPropertyName("expires_in")]
    public long ExpiresIn { get; init; }

    /// <summary>
    /// Token scope (optional — not returned by GigaChat OAuth)
    /// </summary>
    [JsonPropertyName("scope")]
    public string? Scope { get; init; }
}
