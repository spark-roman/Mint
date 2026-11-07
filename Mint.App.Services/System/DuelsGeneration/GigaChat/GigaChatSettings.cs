namespace Mint.App.Services.System.DuelsGeneration.GigaChat;

/// <summary>
/// Settings for GigaChat API
/// </summary>
public record GigaChatSettings
{
    /// <summary>
    /// Configuration section name
    /// </summary>
    public const string SectionName = "GigaChatSettings";

    /// <summary>
    /// GigaChat client ID
    /// </summary>
    public required string ClientId { get; init; } = string.Empty;

    /// <summary>
    /// GigaChat client secret
    /// </summary>
    public required string ClientSecret { get; init; } = string.Empty;

    /// <summary>
    /// API scope (default: GIGACHAT_API_PERS)
    /// </summary>
    public string Scope { get; init; } = "GIGACHAT_API_PERS";

    /// <summary>
    /// Model name for requests (e.g., GigaChat-2)
    /// </summary>
    public required string Model { get; init; } = string.Empty;

    /// <summary>
    /// OAuth token endpoint URL
    /// </summary>
    public Uri AuthUrl { get; init; } = new("https://ngw.devices.sberbank.ru:9443/api/v2/oauth");

    /// <summary>
    /// GigaChat API base URL
    /// </summary>
    public Uri BaseUrl { get; init; } = new("https://api.giga.chat/v1");

    /// <summary>
    /// Request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; init; } = 60;

    /// <summary>
    /// Maximum number of retries for transient errors
    /// </summary>
    public int MaxRetries { get; init; } = 2;

    /// <summary>
    /// Fixed time of day for duel expiration (e.g., 06:30)
    /// </summary>
    public TimeSpan ExpirationTime { get; init; } = new(6, 30, 0);

    /// <summary>
    /// Number of days after which generated duels expire
    /// </summary>
    public int DuelExpirationDays { get; init; } = 2;
}
