using System.Text.Json.Serialization;

namespace Mint.App.Services.System.DuelsGeneration.GigaChat.Dto;

/// <summary>
/// AI generated duel from GigaChat
/// </summary>
public record GigaChatDuelDto
{
    /// <summary>
    /// Duel title with emoji
    /// </summary>
    [JsonPropertyName("Title")]
    public required string Title { get; init; }

    /// <summary>
    /// Description of the conflict
    /// </summary>
    [JsonPropertyName("Description")]
    public required string Description { get; init; }

    /// <summary>
    /// Answer variants
    /// </summary>
    [JsonPropertyName("Options")]
    public required ICollection<string> Options { get; init; }
}
