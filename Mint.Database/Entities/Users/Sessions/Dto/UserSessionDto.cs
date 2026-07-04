using Mint.Database.Entities.Bot.Commands.Dto;

namespace Mint.Database.Entities.Users.Sessions.Dto;

/// <summary>
/// Represents a user session within a scenario
/// </summary>
public record UserSessionDto
{
    /// <summary>Unique session identifier</summary>
    public long Id { get; init; }
    
    /// <summary>User identifier</summary>
    public long UserId { get; init; }
    
    /// <summary>Scenario identifier</summary>
    public long ScenarioId { get; init; }
    
    /// <summary>Current step identifier</summary>
    public long CurrentStepId { get; init; }
    
    /// <summary>JSON data stored for the session</summary>
    public string Data { get; init; } = "{}";
    
    /// <summary>Session start timestamp</summary>
    public DateTimeOffset StartedAt { get; init; }
    
    /// <summary>Session completion timestamp (null if active)</summary>
    public DateTimeOffset? CompletedAt { get; init; }
    
    /// <summary>Current step details (optional)</summary>
    public StepDto? CurrentStep { get; init; }
}