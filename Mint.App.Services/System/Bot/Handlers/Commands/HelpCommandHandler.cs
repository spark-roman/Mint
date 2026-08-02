using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class HelpCommandHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository) : ICommandHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository;
    private readonly IUserSessionRepository _sessionRepository = sessionRepository;

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Help, cancellationToken);
        if (scenario == null)
        {
            return new CommandResult
            {
                Message = "❌ Раздел помощи временно недоступен.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
        {
            return new CommandResult
            {
                Message = "❌ Контент помощи не найден.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        await _sessionRepository.CreateOrUpdateSessionAsync(
            tgUser.Id,
            scenario.Id,
            step.Id,
            "{}",
            cancellationToken);

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        return new CommandResult
        {
            Message = step.Message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = step.IsFinal,
            IsNewMessage = true
        };
    }
}