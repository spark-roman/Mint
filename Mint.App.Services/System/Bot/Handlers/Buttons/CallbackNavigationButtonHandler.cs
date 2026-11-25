using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class CallbackNavigationButtonHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        var button = await _scenarioRepository.GetButtonByActionAsync(callbackData, cancellationToken);
        if (button == null)
            return CommandResult.Error("Неизвестное действие");

        var nextStep = await _scenarioRepository.GetNextStepByButtonIdAsync(button.Id, cancellationToken);
        if (nextStep == null)
            return CommandResult.Error("Следующий шаг не найден");

        var session = await _sessionRepository.GetActiveSessionAsync(externalUserId, cancellationToken);
        if (session != null)
            await _sessionRepository.UpdateCurrentStepAsync(session.Id, nextStep.Id, cancellationToken);

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(nextStep.Id, cancellationToken);

        return new CommandResult
        {
            Message = nextStep.Message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = nextStep.IsFinal,
            IsNewMessage = false
        };
    }
}
