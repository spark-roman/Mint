using System.Collections.ObjectModel;
using Mint.App.Services.System.DuelsGeneration.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Validators;

/// <summary>
/// AI generation validator
/// </summary>
public interface IDuelGenerationValidator
{
    /// <summary>
    /// Validates a duel generation request
    /// </summary>
    /// <param name="duels">Duels to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    GenerationValidationResult Validate(Collection<DuelGenerationDto>? duels);
}
