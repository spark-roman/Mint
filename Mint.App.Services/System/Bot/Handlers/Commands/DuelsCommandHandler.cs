using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Dto;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class DuelsCommandHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    ICategoryRepository categoryRepository,
    IMessageFormatter messageFormatter,
    TimeProvider timeProvider) : ICommandHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository
        ?? throw new ArgumentNullException(nameof(scenarioRepository));

    private readonly IUserSessionRepository _sessionRepository = sessionRepository
       ?? throw new ArgumentNullException(nameof(sessionRepository));

    private readonly ICategoryRepository _categoryRepository = categoryRepository
        ?? throw new ArgumentNullException(nameof(categoryRepository));

    private readonly IMessageFormatter _messageFormatter = messageFormatter
        ?? throw new ArgumentNullException(nameof(messageFormatter));

    private readonly TimeProvider _timeProvider = timeProvider
        ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
        {
            return CommandResult.Error("Сценарий не найден");
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
        {
            return CommandResult.Error("Шаг не найден");
        }

        var categories = await _categoryRepository.GetAllActiveAsync(cancellationToken);
        if (categories.Count == 0)
        {
            return CommandResult.Error("Нет доступных категорий");
        }

        var categoryStatuses = await _categoryRepository.GetCategoriesWithDuelStatusAsync(tgUser.Id, _timeProvider.GetUtcNow(), cancellationToken);

        var categoryButtons = categoryStatuses.Select((cs, index) => new ButtonDto
        {
            Id = index + 1,
            Caption = $"{cs.StatusEmoji} {cs.CategoryName}",
            Action = $"{ActionConstants.CategoryPrefix}{cs.CategoryCode}",
            OrderNum = (short)index
        }).ToList();

        var backToMenuButton = await _scenarioRepository.GetButtonByIdAsync(9, cancellationToken);
        if (backToMenuButton != null)
        {
            backToMenuButton.OrderNum = (short)categoryButtons.Count;
            categoryButtons.Add(backToMenuButton);
        }

        await _sessionRepository.CreateOrUpdateSessionAsync(
            tgUser.Id,
            scenario.Id,
            step.Id,
            $"{{\"step\":\"categories\"}}",
            cancellationToken);

        var stepMessage = await _messageFormatter.FormatCategoriesAsync(step.Message, new Collection<CategoryDto>(categories), cancellationToken);

        return new CommandResult
        {
            Message = stepMessage,
            Keyboard = new Collection<ButtonDto>(categoryButtons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
