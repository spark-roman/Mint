using System.Text.Json.Serialization;

namespace Mint.App.Services.System.DuelsGeneration.Dto;

/// <summary>
/// User response variants
/// </summary>
public record OptionGenerationDto
{
    /// <summary>
    /// Variant code
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;
    
    /// <summary>
    /// Variant text
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; init; } = string.Empty;
}
