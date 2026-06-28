using System.Text.Json.Serialization;

namespace Mint.App.Services.System.DuelsGeneration.Dto;

/// <summary>
/// AI generated duel
/// </summary>
public record DuelGenerationDto
{
    /// <summary>
    /// Category code
    /// </summary>
    [JsonPropertyName("category_code")]
    public string CategoryCode { get; init; } = string.Empty;

    /// <summary>
    /// Duel type
    /// </summary>
    [JsonPropertyName("duel_type")]
    public int DuelType { get; init; }

    /// <summary>
    /// Duel question
    /// </summary>
    [JsonPropertyName("question")]
    public string Question { get; init; } = string.Empty;

    /// <summary>
    /// Duel description
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; init; } = string.Empty;
    
    /// <summary>
    /// Answer variants 
    /// </summary>
    [JsonPropertyName("options")]
    public required ICollection<OptionGenerationDto> Options { get; init; }
}
