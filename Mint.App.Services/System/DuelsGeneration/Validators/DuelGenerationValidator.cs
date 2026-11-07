using System.Collections.ObjectModel;
using Mint.App.Services.System.DuelsGeneration.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Validators;

/// <inheritdoc/>
public class DuelGenerationValidator : IDuelGenerationValidator
{
    /// <inheritdoc/>
    public GenerationValidationResult Validate(Collection<DuelGenerationDto>? duels)
    {
        ArgumentNullException.ThrowIfNull(duels);

        if (duels.Count == 0)
        {
            return new GenerationValidationResult { IsValid = false, Message = "No duels to validate" };
        }
        
        foreach (var duel in duels)
        {
            if (duel.Options.Count < 2 || duel.Options.Count > 4)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Duel '{duel.Question}' has {duel.Options.Count} options, must be 2-4" };
            }

            if (duel.Question.Length > 150)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Question too long: {duel.Question.Length} chars" };
            }

            if (duel.Description.Length > 500)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Description too long: {duel.Description.Length} chars" };
            }

            if (string.IsNullOrEmpty(duel.CategoryCode))
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Category code is empty" };
            }

            var options = duel.Options.ToList();
            if (options.Distinct().Count() != options.Count)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Duplicate options in duel '{duel.Question}'" };
            }

            if (duel.DuelType != 1 && duel.DuelType != 2)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Invalid duel_type {duel.DuelType} in duel '{duel.Question}'" };
            }
        }

        return new GenerationValidationResult { IsValid = true };
    }
}
