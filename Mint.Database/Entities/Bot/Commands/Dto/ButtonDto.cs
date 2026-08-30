using Mint.Common.Contracts.Bot.Commands;

namespace Mint.Database.Entities.Bot.Commands.Dto;

/// <summary>
/// Represents a button within a step
/// </summary>
public record ButtonDto
{
    /// <summary>Unique button identifier</summary>
    public long Id { get; init; }
    
    /// <summary>Parent step identifier</summary>
    public long StepId { get; init; }
    
    /// <summary>Button order number within the step</summary>
    public short OrderNum { get; set; }
    
    /// <summary>Button caption displayed to the user</summary>
    public string Caption { get; init; } = string.Empty;
    
    /// <summary>Action identifier (callback data or special command)</summary>
    public string Action { get; set; } = string.Empty;
    
    /// <summary>Identifier of the next step to navigate to when button is clicked</summary>
    public long? NextStepId { get; init; }

    /// <summary>Button type</summary>
    public TgButtonType Type { get; set; }
}
