using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Users.Dto;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class StartCommandHandler(
    [FromKeyedServices(TgCommandType.MainMenu)] ICommandHandler mainMenuCommandHandler,
    IUserProfilesHandler profileHandler,
    IDtoMapper<User, UserCreateDto> userCreateDtoMapper) : ICommandHandler
{
    private readonly ICommandHandler _mainMenuCommandHandler = mainMenuCommandHandler
        ?? throw new ArgumentNullException(nameof(mainMenuCommandHandler));

    private readonly IUserProfilesHandler _profileHandler = profileHandler
        ?? throw new ArgumentNullException(nameof(profileHandler));

    private readonly IDtoMapper<User, UserCreateDto> _userCreateDtoMapper = userCreateDtoMapper
        ?? throw new ArgumentNullException(nameof(userCreateDtoMapper));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var userCreateDto = _userCreateDtoMapper.Map(tgUser);
        await _profileHandler.InitializeUserAsync(userCreateDto, cancellationToken);

        var commandResult = await _mainMenuCommandHandler.HandleAsync(tgUser, "start", cancellationToken);

        return commandResult;
    }
}