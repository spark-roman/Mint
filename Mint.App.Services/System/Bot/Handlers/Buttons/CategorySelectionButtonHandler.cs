using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Duels.Handlers;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Settings;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class CategorySelectionButtonHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    ICategoryRepository categoryRepository,
    IAccountRepository accountRepository,
    IMessageFormatter messageFormatter,
    IDuelHandler duelHandler) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));
    private readonly IDuelHandler _duelHandler = duelHandler ?? throw new ArgumentNullException(nameof(duelHandler));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        var categoryCode = callbackData[ActionConstants.CategoryPrefix.Length..];
        var category = await _categoryRepository.GetByCodeAsync(categoryCode, cancellationToken);
        if (category == null)
            return CommandResult.Error("Категория не найдена");

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
            return CommandResult.Error("Сценарий не найден");

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, cancellationToken);
        if (step == null)
            return CommandResult.Error("Шаг дуэли не найден");

        var userAccount = await _accountRepository.GetAccountByExternalUserIdAsync(externalUserId, (byte)Common.Contracts.Users.AuthSystem.Tg, cancellationToken)
            ?? throw new InvalidOperationException("User account not found");

        var duelCard = await _duelHandler.GetFirstAvailableDuelAsync(category.Id, userAccount.Id, cancellationToken);
        if (duelCard == null)
        {
            return new CommandResult
            {
                Message = "😕 В этой категории пока нет активных дуэлей.",
                Keyboard = new Collection<ButtonDto>
                {
                    new() { Caption = "⬅️ Назад", Action = ActionConstants.Duels }
                },
                IsFinal = false,
                IsNewMessage = false
            };
        }

        var message = await _messageFormatter.FormatDuelAsync(step.Message, duelCard, cancellationToken);

        var optionButtons = duelCard.Options.Select((o, index) => new ButtonDto
        {
            Caption = o.OptionText,
            Action = $"{ActionConstants.VotePrefix}{duelCard.DuelId}_{o.Id}",
            OrderNum = (short)index
        }).ToList();

        optionButtons.Add(new ButtonDto
        {
            Caption = "🔗 Поспорить с другом",
            Action = $"{ActionConstants.SharePrefix}{duelCard.DuelId}",
            OrderNum = (short)optionButtons.Count
        });

        var backToDuelsButton = await _scenarioRepository.GetButtonByIdAsync(10, cancellationToken);
        backToDuelsButton!.OrderNum = (short)(optionButtons.Count + 1);
        optionButtons.Add(backToDuelsButton);

        await _sessionRepository.CreateOrUpdateSessionAsync(
            externalUserId,
            scenario.Id,
            step.Id,
            $"{{\"step\":\"duel\",\"duel_id\":{duelCard.DuelId}}}",
            cancellationToken);

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(optionButtons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
