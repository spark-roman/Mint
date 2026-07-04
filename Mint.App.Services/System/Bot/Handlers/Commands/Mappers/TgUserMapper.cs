using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Common.Contracts.Mappers;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands.Mappers;

/// <inheritdoc/>
public class TgUserMapper : IDtoMapper<User, ExternalUserDto>
{
    /// <inheritdoc/>
    public ExternalUserDto Map(User dto, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new ExternalUserDto
        {
            ExternalUserId = dto.Id,
            FirstName = dto.FirstName,
            Username = dto.Username,
            LastName = dto.LastName,
            IsPremium = dto.IsPremium,
            IsBot = dto.IsBot,
            AddedToAttachmentMenu = dto.AddedToAttachmentMenu,
            CanJoinGroups = dto.CanJoinGroups,
            CanReadAllGroupMessages = dto.CanReadAllGroupMessages,
            SupportsInlineQueries = dto.SupportsInlineQueries
        };
    }
}
