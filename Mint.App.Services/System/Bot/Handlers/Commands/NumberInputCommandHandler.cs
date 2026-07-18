using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Duels.Dto;
using Mint.App.Services.UserInteractive.Duels.Handlers;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc />
public class NumberInputCommandHandler(
    IUserSessionRepository sessionRepository,
    IDuelHandler duelHandler,
    IDuelRepository duelRepository,
    IScenarioRepository scenarioRepository,
    IMessageFormatter messageFormatter,
    ILogger<NumberInputCommandHandler> logger) : ICommandHandler
{
    private readonly IUserSessionRepository _sessionRepository = sessionRepository
        ?? throw new ArgumentNullException(nameof(sessionRepository));

    private readonly IDuelHandler _duelHandler = duelHandler
        ?? throw new ArgumentNullException(nameof(duelHandler));

    private readonly IDuelRepository _duelRepository = duelRepository
        ?? throw new ArgumentNullException(nameof(duelRepository));

    private readonly IScenarioRepository _scenarioRepository = scenarioRepository
        ?? throw new ArgumentNullException(nameof(scenarioRepository));
    
    private readonly IMessageFormatter _messageFormatter = messageFormatter
        ?? throw new ArgumentNullException(nameof(messageFormatter));

    private readonly ILogger<NumberInputCommandHandler> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        _logger.LogInformation("NumberInputHandler: processing '{Input}' for user {UserId}", inputData, tgUser.Id);

        if (!decimal.TryParse(inputData, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount) || amount <= 0)
        {
            return new CommandResult
            {
                Message = "❌ Пожалуйста, введите положительное число.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var session = await _sessionRepository.GetActiveSessionAsync(tgUser.Id, cancellationToken);
        if (session == null)
        {
            return new CommandResult
            {
                Message = "❌ Сессия не найдена. Пожалуйста, выберите категорию заново.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        _logger.LogInformation("Session found: {SessionId}, Data: {Data}", session.Id, session.Data);

        Dictionary<string, object>? sessionData;

        try
        {
            sessionData = JsonSerializer.Deserialize<Dictionary<string, object>>(session.Data);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize session data");
            return new CommandResult
            {
                Message = "❌ Ошибка чтения данных сессии. Попробуйте начать заново.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        if (sessionData == null ||
            !sessionData.TryGetValue("duel_id", out var duelIdValue) ||
            !sessionData.TryGetValue("option_id", out var optionIdValue))
        {
            return new CommandResult
            {
                Message = "❌ Данные о дуэли не найдены. Пожалуйста, выберите вариант ответа заново.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var duelId = ((JsonElement)duelIdValue).GetInt64();
        var optionId = ((JsonElement)optionIdValue).GetInt64();

        _logger.LogInformation("DuelId: {DuelId}, OptionId: {OptionId}, Amount: {Amount}", duelId, optionId, amount);

        var result = await _duelHandler.PlaceBetAsync(tgUser.Id, duelId, optionId, amount, cancellationToken);

        if (!result.Success)
        {
            return new CommandResult
            {
                Message = $"❌ {result.Message}",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var duel = await _duelRepository.GetDuelByIdAsync(duelId, cancellationToken);
        
        if (duel == null)
        {
            return CommandResult.Error("Дуэль не найдена");
        }

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
        {
            return CommandResult.Error("Сценарий не найден");
        }

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 4, cancellationToken);
        if (step == null)
        {
            return CommandResult.Error("Шаг успеха не найден");
        }

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

        await _sessionRepository.UpdateCurrentStepAsync(session.Id, step.Id, cancellationToken);

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}