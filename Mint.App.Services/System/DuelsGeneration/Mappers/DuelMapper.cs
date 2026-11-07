using System.Globalization;
using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Mappers;

/// <inheritdoc />
public class DuelMapper : IDtoMapper<DuelGenerationDto, DuelCreateDto>
{
    /// <inheritdoc />
    public DuelCreateDto Map(DuelGenerationDto dto, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(args);
        
        var categoryId = (int)args[0];
        var expiresAt = (DateTimeOffset)args[1];
        
        var duelType = dto.DuelType switch
        {
            1 => DuelType.OpinionMatch,
            2 => DuelType.FactPrediction,
            _ => DuelType.None
        };

        return new DuelCreateDto
        {
            CategoryId = categoryId,
            DuelType = duelType,
            Question = dto.Question,
            Description = dto.Description,
            ExpiresAt = expiresAt,
            Options = [..dto.Options.Select((opt, index) =>
            new DuelOptionCreateDto
            {
                OptionText = opt,
                OptionCode = index.ToString(CultureInfo.InvariantCulture)
            })]
        };
    }
}
