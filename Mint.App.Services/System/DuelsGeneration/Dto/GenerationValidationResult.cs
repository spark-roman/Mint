namespace Mint.App.Services.System.DuelsGeneration.Dto;

/// <summary>
/// Validation result of a generation.
/// </summary>
public record GenerationValidationResult
{
    /// <summary>
    /// True if the generation is valid, false otherwise.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Optional message about the validation result.
    /// </summary>
    public string? Message { get; set; }
}
