using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.System.Settings.Handlers;
using Mint.App.Services.UserInteractive.Duels.Dto;
using Mint.App.Services.UserInteractive.Duels.Handlers;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Settings;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class BetButtonHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    IDuelRepository duelRepository,
    IDuelHandler duelHandler,
    ISystemSettingHandler systemSettingHandler,
    IMessageFormatter messageFormatter) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    private readonly IDuelRepository _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));
    private readonly IDuelHandler _duelHandler = duelHandler ?? throw new ArgumentNullException(nameof(duelHandler));
    private readonly ISystemSettingHandler _systemSettingHandler = systemSettingHandler ?? throw new ArgumentNullException(nameof(systemSettingHandler));
    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        var parts = callbackData.Split('_');
        if (parts.Length != 4 || parts[0] != ActionConstants.BetPrefix.TrimEnd('_'))
            return CommandResult.Error("Неверный формат");

        if (!long.TryParse(parts[1], out var duelId) || !long.TryParse(parts[2], out var optionId) || !decimal.TryParse(parts[3], out var amount))
            return CommandResult.Error("Неверные данные");

        var minBet = await _systemSettingHandler.GetIntAsync(SettingKeysConstants.MinBetAmount, 10, cancellationToken);
        var maxBet = await _systemSettingHandler.GetIntAsync(SettingKeysConstants.MaxBetAmount, 10000, cancellationToken);

        if (amount < minBet)
            return CommandResult.Error($"Минимальная сумма ставки — {minBet}.");

        if (amount > maxBet)
            return CommandResult.Error($"Максимальная сумма ставки — {maxBet}.");

        var result = await _duelHandler.PlaceBetAsync(externalUserId, duelId, optionId, amount, cancellationToken);
        if (!result.Success)
            return CommandResult.Error(result.Message);

        var duel = await _duelRepository.GetDuelByIdAsync(duelId, cancellationToken);
        if (duel == null)
            return CommandResult.Error("Дуэль не найдена");

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
            return CommandResult.Error("Сценарий не найден");

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 4, cancellationToken);
        if (step == null)
            return CommandResult.Error("Шаг успеха не найден");

        var successData = new SuccessDataDto
        {
            DuelId = duelId,
            SelectedOption = duel.Options.FirstOrDefault(o => o.Id == optionId)?.OptionText ?? "Неизвестно",
            BetAmount = amount,
            ExpiresAt = duel.ExpiresAt
        };

        var message = await _messageFormatter.FormatSuccessAsync(step.Message, successData, cancellationToken);

        var buttons = new List<ButtonDto>
        {
            new() { Caption = "✉️ Переслать друзьям", Action = $"{ActionConstants.SharePrefix}duel_{duelId}" },
            new() { Caption = "📊 Следующая дуэль", Action = ActionConstants.Duels },
            new() { Caption = "⬅️ Назад в меню", Action = ActionConstants.MainMenu }
        };

        await _sessionRepository.CreateOrUpdateSessionAsync(
            externalUserId,
            scenario.Id,
            step.Id,
            $"{{\"step\":\"success\",\"duel_id\":{duelId}}}",
            cancellationToken);

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
