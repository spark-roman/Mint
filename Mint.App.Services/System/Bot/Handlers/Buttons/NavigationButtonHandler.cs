using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Settings;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class NavigationButtonHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        var scenarioName = callbackData switch
        {
            ActionConstants.MainMenu => ScenarioConstants.Start,
            ActionConstants.Profile => ScenarioConstants.Profile,
            ActionConstants.Duels => ScenarioConstants.Duels,
            ActionConstants.Referral => ScenarioConstants.Referral,
            ActionConstants.Leaderboard => ScenarioConstants.Leaderboard,
            _ => null
        };

        if (scenarioName == null)
            return CommandResult.Error("Неизвестное действие");

        return await NavigateToScenarioAsync(externalUserId, scenarioName, cancellationToken);
    }

    private async Task<CommandResult> NavigateToScenarioAsync(long externalUserId, string scenarioName, CancellationToken cancellationToken)
    {
        var scenario = await _scenarioRepository.GetScenarioByNameAsync(scenarioName, cancellationToken);
        if (scenario == null)
            return CommandResult.Error($"Сценарий '{scenarioName}' не найден");

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
            return CommandResult.Error("Первый шаг не найден");

        await _sessionRepository.CreateOrUpdateSessionAsync(externalUserId, scenario.Id, step.Id, "{}", cancellationToken);

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        return new CommandResult
        {
            Message = step.Message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
