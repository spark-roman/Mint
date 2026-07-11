using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Microsoft.VisualBasic;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Leaderboards;
using Mint.App.Services.UserInteractive.Profiles.Dto;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class LeaderboardCommandHandler(
    ILeaderboardHandler leaderboardHandler,
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    IMessageFormatter messageFormatter) : ICommandHandler
{
    private readonly ILeaderboardHandler _leaderboardHandler = leaderboardHandler ?? throw new ArgumentNullException(nameof(leaderboardHandler));

    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));

    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));

    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

    private const int DefaultTop = 15;

    /// <inheritdoc/>
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var top = ParseTop(inputData);

        var result = await _leaderboardHandler.GetLeaderboardAsync(top, tgUser.Id, AuthSystem.Tg, cancellationToken);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync("profile", cancellationToken);
        if (scenario == null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: сценарий 'profile' не найден",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, cancellationToken);

        if (step == null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: шаг лидерборда не найден",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var message = await _messageFormatter.FormatLeaderboardAsync(step.Message, result, cancellationToken);

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        await _sessionRepository.CreateOrUpdateSessionAsync(
            tgUser.Id,
            scenario.Id,
            step.Id,
            $"{{\"step\":\"leaderboard\",\"top\":{top}}}",
            cancellationToken);

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = false,
            IsNewMessage = false
        };
    }

    private static int ParseTop(string inputData)
    {
        if (string.IsNullOrEmpty(inputData))
            return DefaultTop;

        if (int.TryParse(inputData, out var top))
            return Math.Max(1, top);

        return DefaultTop;
    }
}
