using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public class MainMenuCommandHandler(
    IUserProfilesHandler profileHandler,
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    IMessageFormatter messageFormatter) : ICommandHandler
{
    private readonly IUserProfilesHandler _profileHandler = profileHandler
        ?? throw new ArgumentNullException(nameof(profileHandler));

    private readonly IScenarioRepository _scenarioRepository = scenarioRepository
        ?? throw new ArgumentNullException(nameof(scenarioRepository));

    private readonly IUserSessionRepository _sessionRepository = sessionRepository
        ?? throw new ArgumentNullException(nameof(sessionRepository));

    private readonly IMessageFormatter _messageFormatter = messageFormatter
        ?? throw new ArgumentNullException(nameof(messageFormatter));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

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

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);
        var profile = await _profileHandler.GetProfileAsync(tgUser.Id, AuthSystem.Tg, cancellationToken);

        var text = await _messageFormatter.FormatAsync(step.Message, profile, cancellationToken);

        var isNewMessage = inputData == "start";

        return new CommandResult
        {
            Message = text,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = step.IsFinal,
            IsNewMessage = isNewMessage
        };
    }
}
