using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Dto;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class StartCommandHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    IUserProfilesHandler profileHandler,
    IDtoMapper<User, UserCreateDto> userCreateDtoMapper) : ICommandHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));

    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));

    private readonly IUserProfilesHandler _profileHandler = profileHandler ?? throw new ArgumentNullException(nameof(profileHandler));

    private readonly IDtoMapper<User, UserCreateDto> _userCreateDtoMapper = userCreateDtoMapper ?? throw new ArgumentNullException(nameof(userCreateDtoMapper));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var userCreateDto = _userCreateDtoMapper.Map(tgUser);
        await _profileHandler.InitializeUserAsync(userCreateDto, cancellationToken);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync("start", cancellationToken);
        if (scenario == null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: сценарий 'start' не найден",
                IsFinal = true
            };
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: первый шаг не найден",
                IsFinal = true
            };
        }

        await _sessionRepository.CreateOrUpdateSessionAsync(
            tgUser.Id,
            scenario.Id,
            step.Id,
            "{}",
            cancellationToken);

        /*var externalUser = _userMapper.Map(tgUser);
        var userProfile = await _profileHandler.GetUserProfileAsync(externalUser, cancellationToken);

        if (userProfile is null)
        {
            throw new InvalidOperationException($"Profile not found: {externalUser.ExternalUserId}");
        }

        var message = await _messageFormatter.FormatAsync(step.Message, userProfile, cancellationToken);*/

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        return new CommandResult
        {
            Message = step.Message,
            Keyboard = buttons.AsReadOnly(),
            IsFinal = step.IsFinal,
            IsNewMessage = true
        };
    }
}