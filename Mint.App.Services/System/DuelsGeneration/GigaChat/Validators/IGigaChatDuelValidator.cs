using System.Collections.ObjectModel;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.GigaChat.Dto;

namespace Mint.App.Services.System.DuelsGeneration.GigaChat.Validators;

/// <summary>
/// Validates GigaChat duel generation results
/// </summary>
public interface IGigaChatDuelValidator
{
    /// <summary>
    /// Validates a duel generation result
    /// </summary>
    /// <param name="duels">Duels to validate</param>
    /// <returns>Validation result</returns>
    GenerationValidationResult Validate(Collection<GigaChatDuelDto>? duels);
}
