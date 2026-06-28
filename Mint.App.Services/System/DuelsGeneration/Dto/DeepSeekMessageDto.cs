using System.Text.Json.Serialization;

namespace Mint.App.Services.System.DuelsGeneration.Dto;

/// <summary>
/// Deepseek message
/// </summary>
public record DeepSeekMessageDto
{
    /// <summary>
    /// Message content
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}
