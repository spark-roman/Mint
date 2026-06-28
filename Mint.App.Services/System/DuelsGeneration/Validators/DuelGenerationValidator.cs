using System.Collections.ObjectModel;
using Mint.App.Services.System.DuelsGeneration.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Validators;

/// <inheritdoc/>
public class DuelGenerationValidator : IDuelGenerationValidator
{
    private static readonly string[] _options = ["a", "b", "c", "d"];

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

            var codes = duel.Options.Select(o => o.Code).ToList();
            if (codes.Distinct().Count() != codes.Count)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Duplicate option codes in duel '{duel.Question}'" };
            }
                
            foreach (var code in codes)
            {
                if (!_options.Contains(code))
                {
                    return new GenerationValidationResult { IsValid = false, Message = $"Invalid option code '{code}' in duel '{duel.Question}'" };
                }
            }

            if (duel.DuelType != 1 && duel.DuelType != 2)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Invalid duel_type {duel.DuelType} in duel '{duel.Question}'" };
            }
        }

        return new GenerationValidationResult { IsValid = true };
    }
}
