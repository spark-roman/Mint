using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.Common.Contracts.Mappers;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands.Mappers;

/// <inheritdoc/>
public class TgCommandMapper : IDtoMapper<Update, UpdateCommandDto>
{
    /// <inheritdoc/>
    public UpdateCommandDto Map(Update dto, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var updateCommand = dto.CallbackQuery is not null
            ? new UpdateCommandDto
            {
                CommandText = string.Empty,
                CallbackData = dto.CallbackQuery.Data,
                CallbackId = dto.CallbackQuery.Id,
                ChatId = dto.CallbackQuery.Message!.Chat.Id,
                User = dto.CallbackQuery.From,
                MessageId = dto.CallbackQuery.Message.MessageId
            }
            : new UpdateCommandDto
            {
                CommandText = dto.Message!.Text,
                CallbackData = string.Empty,
                CallbackId = string.Empty,
                ChatId = dto.Message.Chat.Id,
                User = dto.Message.From,
                MessageId = dto.Message.MessageId
            };
            
        return updateCommand;
    }
}
