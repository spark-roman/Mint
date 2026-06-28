using System.Collections.ObjectModel;
using System.Text.Json;
using System.Linq;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.App.Services.System.DuelsGeneration.Validators;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Processors;

/// <summary>
/// Processes AI responses from Deepseek API into duel creation DTOs.
/// </summary>
public class DeepseekResponseProcessor : IAIResponseProcessor
{
    private readonly IDtoMapper<DuelGenerationDto, DuelCreateDto> _duelMapper;
    private readonly IDuelGenerationValidator _duelGenerationValidator;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeepseekResponseProcessor"/> class.
    /// </summary>
    /// <param name="duelMapper">Duel mapper.</param>
    /// <param name="duelGenerationValidator">Duel generation validator.</param>
    public DeepseekResponseProcessor(
        IDtoMapper<DuelGenerationDto, DuelCreateDto> duelMapper,
        IDuelGenerationValidator duelGenerationValidator)
    {
        _duelMapper = duelMapper ?? throw new ArgumentNullException(nameof(duelMapper));
        _duelGenerationValidator = duelGenerationValidator ?? throw new ArgumentNullException(nameof(duelGenerationValidator));
    }

    /// <summary>
    /// Processes an AI response into a list of duel creation DTOs.
    /// </summary>
    /// <param name="response">The AI response content containing JSON-serialized duels.</param>
    /// <param name="categoryId">The category ID for the duels.</param>
    /// <param name="daysToLive">Number of days the duels should live.</param>
    /// <returns>A list of duel creation DTOs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when response is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when validation fails.</exception>
    public async Task<List<DuelCreateDto>> Process(string response, int categoryId, int daysToLive)
    {
        ArgumentNullException.ThrowIfNull(response);

        var duelsData = JsonSerializer.Deserialize<List<DuelGenerationDto>>(response);

        var duelsList = duelsData ?? [];
        var collection = new Collection<DuelGenerationDto>(duelsList);
        var validationResult = _duelGenerationValidator.Validate(collection);

        if (!validationResult.IsValid)
        {
            throw new InvalidOperationException($"Duels generation validation failed: {validationResult.Message}");
        }

        var duelsCreateDtos = collection.Select(d => _duelMapper.Map(d, categoryId, daysToLive)).ToList();

        return await Task.FromResult(duelsCreateDtos);
    }
}
