using Mint.App.Services.System.DuelsGeneration.Dto;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.System.DuelsGeneration.Mappers;

/// <inheritdoc />
public class DuelMapper(TimeProvider timeProvider) : IDtoMapper<DuelGenerationDto, DuelCreateDto>
{
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc />
    public DuelCreateDto Map(DuelGenerationDto dto, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(args);
        
        var categoryId = (int)args[0];
        var daysToLive = (int)args[1];

        var expiresAt = _timeProvider.GetUtcNow().AddDays(daysToLive);
        
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
                OptionText = opt.Text,
                OptionCode = opt.Code
            })]
        };
    }
}
