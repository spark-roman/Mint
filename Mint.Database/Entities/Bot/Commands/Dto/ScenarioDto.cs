namespace Mint.Database.Entities.Bot.Commands.Dto;

/// <summary>
/// Represents a bot scenario (e.g., start, profile, duels)
/// </summary>
public record ScenarioDto
{
    /// <summary>Unique scenario identifier</summary>
    public long Id { get; init; }
    
    /// <summary>Scenario name (start, profile, duels, referral)</summary>
    public string Name { get; init; } = string.Empty;
    
    /// <summary>Indicates whether the scenario is active</summary>
    public bool IsActive { get; init; }
    
    /// <summary>Creation timestamp</summary>
    public DateTimeOffset CreatedAt { get; init; }
    
    /// <summary>Collection of steps within this scenario</summary>
    public IReadOnlyCollection<StepDto> Steps { get; init; } = null!;
}