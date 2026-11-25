using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Duels.Dto;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class CancelButtonHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    IDuelRepository duelRepository,
    ICategoryRepository categoryRepository,
    IMessageFormatter messageFormatter) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    private readonly IDuelRepository _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        var parts = callbackData.Split('_');
        if (parts.Length != 2 || parts[0] != ActionConstants.CancelPrefix.TrimEnd('_'))
            return CommandResult.Error("Неверный формат");

        if (!long.TryParse(parts[1], out var duelId))
            return CommandResult.Error("Неверные данные");

        var session = await _sessionRepository.GetActiveSessionAsync(externalUserId, cancellationToken);
        if (session == null)
            return CommandResult.Error("Сессия не найдена");

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
            return CommandResult.Error("Сценарий не найден");

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, cancellationToken);
        if (step == null)
            return CommandResult.Error("Шаг дуэли не найден");

        await _sessionRepository.UpdateCurrentStepAsync(session.Id, step.Id, cancellationToken);

        var duel = await _duelRepository.GetDuelByIdAsync(duelId, cancellationToken);
        if (duel == null)
            return CommandResult.Error("Дуэль не найдена");

        var category = await _categoryRepository.GetByIdAsync(duel.CategoryId, cancellationToken);

        var duelCard = new DuelCardDto
        {
            DuelId = duel.Id,
            CategoryName = category?.Name ?? "Неизвестно",
            Question = duel.Question,
            Description = duel.Description,
            ExpiresAt = duel.ExpiresAt,
            Options = [.. duel.Options.Select(o => new DuelOptionDto
            {
                Id = o.Id,
                OptionText = o.OptionText,
                OptionCode = o.OptionCode
            })]
        };

        var message = await _messageFormatter.FormatDuelAsync(step.Message, duelCard, cancellationToken);

        var optionButtons = duel.Options.Select((o, index) => new ButtonDto
        {
            Caption = o.OptionText,
            Action = $"{ActionConstants.VotePrefix}{duel.Id}_{o.Id}",
            OrderNum = (short)index
        }).ToList();

        optionButtons.Add(new ButtonDto
        {
            Caption = "🔗 Поспорить с другом",
            Action = $"{ActionConstants.SharePrefix}{duel.Id}",
            OrderNum = (short)optionButtons.Count
        });

        optionButtons.Add(new ButtonDto
        {
            Caption = "🔙 К дуэлям",
            Action = $"duels",
            OrderNum = (short)(optionButtons.Count + 1)
        });

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(optionButtons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
