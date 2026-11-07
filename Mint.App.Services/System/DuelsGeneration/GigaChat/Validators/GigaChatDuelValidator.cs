using System.Collections.ObjectModel;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.GigaChat.Dto;

namespace Mint.App.Services.System.DuelsGeneration.GigaChat.Validators;

/// <inheritdoc/>
public class GigaChatDuelValidator : IGigaChatDuelValidator
{
    /// <inheritdoc/>
    public GenerationValidationResult Validate(Collection<GigaChatDuelDto>? duels)
    {
        ArgumentNullException.ThrowIfNull(duels);

        if (duels.Count == 0)
        {
            return new GenerationValidationResult { IsValid = false, Message = "No duels to validate" };
        }

        foreach (var duel in duels)
        {
            if (string.IsNullOrEmpty(duel.Title))
            {
                return new GenerationValidationResult { IsValid = false, Message = "Duel title is empty" };
            }

            if (duel.Title.Length > 150)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Title too long: {duel.Title.Length} chars" };
            }

            if (string.IsNullOrEmpty(duel.Description))
            {
                return new GenerationValidationResult { IsValid = false, Message = "Duel description is empty" };
            }

            if (duel.Description.Length > 2000)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Description too long: {duel.Description.Length} chars" };
            }

            if (duel.Options.Count < 2 || duel.Options.Count > 4)
            {
                return new GenerationValidationResult { IsValid = false, Message = $"Duel '{duel.Title}' has {duel.Options.Count} options, must be 2-4" };
            }

            foreach (var option in duel.Options)
            {
                if (string.IsNullOrEmpty(option))
                {
                    return new GenerationValidationResult { IsValid = false, Message = $"Empty option in duel '{duel.Title}'" };
                }
            }
        }

        return new GenerationValidationResult { IsValid = true };
    }
}
