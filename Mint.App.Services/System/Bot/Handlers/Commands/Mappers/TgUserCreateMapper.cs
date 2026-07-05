using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Users.Dto;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands.Mappers;

/// <inheritdoc/>
public class TgUserCreateMapper(TimeProvider timeProvider) : IDtoMapper<User, UserCreateDto>
{
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc/>
    public UserCreateDto Map(User dto, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new UserCreateDto
        {
            ExternalUserId = dto.Id,
            SystemType = (byte)AuthSystem.Tg,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            UserName = dto.Username,
            CreatedAt = _timeProvider.GetUtcNow(),
        };
    }
}
