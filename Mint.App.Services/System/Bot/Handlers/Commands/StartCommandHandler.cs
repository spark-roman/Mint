using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Users.Dto;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class StartCommandHandler(
    [FromKeyedServices(TgCommandType.MainMenu)] ICommandHandler mainMenuCommandHandler,
    IUserProfilesHandler profileHandler,
    IDtoMapper<User, UserCreateDto> userCreateDtoMapper,
    ILogger<StartCommandHandler> logger) : ICommandHandler
{
    private readonly ICommandHandler _mainMenuCommandHandler = mainMenuCommandHandler
        ?? throw new ArgumentNullException(nameof(mainMenuCommandHandler));

    private readonly IUserProfilesHandler _profileHandler = profileHandler
        ?? throw new ArgumentNullException(nameof(profileHandler));

    private readonly IDtoMapper<User, UserCreateDto> _userCreateDtoMapper = userCreateDtoMapper
        ?? throw new ArgumentNullException(nameof(userCreateDtoMapper));

    private readonly ILogger<StartCommandHandler> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var userCreateDto = _userCreateDtoMapper.Map(tgUser);
        await _profileHandler.InitializeUserAsync(userCreateDto, cancellationToken);

        _logger.LogInformation("Start command with input: {InputData}", inputData);
        if (!string.IsNullOrEmpty(inputData) && inputData.StartsWith("/start", StringComparison.InvariantCultureIgnoreCase))
        {
            var parts = inputData.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                string? referralCode = parts[1];

                _logger.LogInformation("Referral code extracted: {ReferralCode}", referralCode);

                if (!string.IsNullOrEmpty(referralCode))
                {
                    await _profileHandler.ProcessReferralAsync(tgUser.Id, AuthSystem.Tg, referralCode, cancellationToken);
                }
            }
        }

        var commandResult = await _mainMenuCommandHandler.HandleAsync(tgUser, "start", cancellationToken);

        return commandResult;
    }
}