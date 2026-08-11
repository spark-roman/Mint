using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.System.Settings.Handlers;
using Mint.App.Services.UserInteractive.Leaderboards;
using Mint.Common.Contracts.Settings;
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
    IMessageFormatter messageFormatter,
    ISystemSettingHandler systemSettingHandler) : ICommandHandler
{
    private readonly ILeaderboardHandler _leaderboardHandler = leaderboardHandler ?? throw new ArgumentNullException(nameof(leaderboardHandler));

    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));

    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));

    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

    private readonly ISystemSettingHandler _systemSettingHandler = systemSettingHandler ?? throw new ArgumentNullException(nameof(systemSettingHandler));

    /// <inheritdoc/>
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var top = await ParseTop(inputData, cancellationToken);

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

    private async Task<int> ParseTop(string inputData, CancellationToken cancellationToken)
    {
        var defaultTop = await _systemSettingHandler.GetIntAsync(SettingKeysConstants.LeaderboardSize, 15, cancellationToken);

        if (string.IsNullOrEmpty(inputData))
            return defaultTop;

        if (int.TryParse(inputData, out var top))
            return Math.Max(1, top);

        return defaultTop;
    }
}
