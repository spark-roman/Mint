using Mint.App.Services.System.DuelsGeneration.GigaChat.Dto;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Duels.Dto;

namespace Mint.App.Services.System.DuelsGeneration.GigaChat.Mappers;

/// <inheritdoc />
public class GigaChatDuelMapper : IDtoMapper<GigaChatDuelDto, DuelCreateDto>
{
    private static readonly string[] _optionCodes = ["a", "b", "c", "d"];

    /// <inheritdoc />
    public DuelCreateDto Map(GigaChatDuelDto dto, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(args);

        var categoryId = (int)args[0];
        var expiresAt = (DateTimeOffset)args[1];

        var options = dto.Options
            .Select((text, index) => new DuelOptionCreateDto
            {
                OptionText = text,
                OptionCode = index < _optionCodes.Length ? _optionCodes[index] : $"opt{index + 1}"
            })
            .ToList();

        return new DuelCreateDto
        {
            CategoryId = categoryId,
            DuelType = DuelType.OpinionMatch,
            Question = dto.Title,
            Description = dto.Description,
            ExpiresAt = expiresAt,
            Options = [.. options]
        };
    }
}
