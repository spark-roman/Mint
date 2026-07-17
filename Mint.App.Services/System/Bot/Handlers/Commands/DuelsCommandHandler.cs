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
    IMessageFormatter messageFormatter) : ICommandHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository
        ?? throw new ArgumentNullException(nameof(scenarioRepository));

    private readonly IUserSessionRepository _sessionRepository = sessionRepository
       ?? throw new ArgumentNullException(nameof(sessionRepository));

    private readonly ICategoryRepository _categoryRepository = categoryRepository
        ?? throw new ArgumentNullException(nameof(categoryRepository));

    private readonly IMessageFormatter _messageFormatter = messageFormatter
        ?? throw new ArgumentNullException(nameof(messageFormatter));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
        {
            return new CommandResult
            {
                Message = "❌ Сценарий не найден",
                IsFinal = true
            };
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
        {
            return new CommandResult
            {
                Message = "❌ Шаг не найден",
                IsFinal = true
            };
        }

        var categories = await _categoryRepository.GetAllActiveAsync(cancellationToken);
        if (categories.Count == 0)
        {
            return CommandResult.Error("Нет доступных категорий");
        }

        var backToDuelsButton = await _scenarioRepository.GetButtonByIdAsync(9, cancellationToken);

        var categoryButtons = categories.Select((c, index) => new ButtonDto
        {
            Id = index + 1,
            Caption = $"📂 {c.Name}",
            Action = $"{ActionConstants.CategoryPrefix}{c.Code}",
            OrderNum = (short)categories.ToList().IndexOf(c)
        })
        .OrderBy(b => b.Id)
        .ToList();

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
            Keyboard = new Collection<ButtonDto>([..categoryButtons, backToDuelsButton!]),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
