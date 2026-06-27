namespace Mint.App.Services.System.DuelsGeneration.Dto;

/// <summary>
/// Settings for DeepSeek API
/// </summary>
public record DeepSeekSettings
{
    /// <summary>
    /// DeepSeek API URL
    /// </summary>
    public required Uri DeepSeekApiUrl { get; init; }

    /// <summary>
    /// DeepSeek API token
    /// </summary>
    public required string Token { get; init; }
}
