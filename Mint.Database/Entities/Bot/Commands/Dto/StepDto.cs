namespace Mint.Database.Entities.Bot.Commands.Dto;

/// <summary>
/// Represents a single step within a scenario
/// </summary>
public record StepDto
{
    /// <summary>Unique step identifier</summary>
    public long Id { get; init; }
    
    /// <summary>Parent scenario identifier</summary>
    public long ScenarioId { get; init; }
    
    /// <summary>Step order number within the scenario</summary>
    public short OrderNum { get; init; }
    
    /// <summary>Step type identifier (text, number, choice, info)</summary>
    public short StepTypeId { get; init; }
    
    /// <summary>Message template with placeholders (e.g., {{balance}})</summary>
    public string Message { get; init; } = string.Empty;
    
    /// <summary>Indicates whether this step is the final one</summary>
    public bool IsFinal { get; init; }
    
    /// <summary>Additional JSON data for the step</summary>
    public string? Data { get; init; }
    
    /// <summary>Collection of buttons available on this step</summary>
    public IReadOnlyCollection<ButtonDto> Buttons { get; init; } = null!;
}
