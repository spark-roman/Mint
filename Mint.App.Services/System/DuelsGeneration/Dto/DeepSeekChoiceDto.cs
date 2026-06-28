using System.Text.Json.Serialization;

namespace Mint.App.Services.System.DuelsGeneration.Dto;

/// <summary>
/// Choice
/// </summary>
public record DeepSeekChoiceDto
{
    /// <summary>
    /// Message
    /// </summary>
    [JsonPropertyName("message")]
    public required DeepSeekMessageDto Message { get; init; }
}
