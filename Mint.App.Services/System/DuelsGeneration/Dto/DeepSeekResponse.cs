using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace Mint.App.Services.System.DuelsGeneration.Dto;

/// <summary>
/// Deepsek response
/// </summary>
public record DeepSeekResponse
{
    /// <summary>
    /// Choices
    /// </summary>
    [JsonPropertyName("choices")]
    public required Collection<DeepSeekChoiceDto> Choices { get; init; }
}
