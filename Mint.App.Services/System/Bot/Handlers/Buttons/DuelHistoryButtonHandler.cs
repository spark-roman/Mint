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
using Mint.Common.Contracts.Users;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class DuelHistoryButtonHandler(
    IScenarioRepository scenarioRepository,
    IAccountRepository accountRepository,
    IDuelHistoryHandler duelHistoryHandler,
    IMessageFormatter messageFormatter) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    private readonly IDuelHistoryHandler _duelHistoryHandler = duelHistoryHandler ?? throw new ArgumentNullException(nameof(duelHistoryHandler));
    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        var userAccount = await _accountRepository.GetAccountByExternalUserIdAsync(externalUserId, (byte)AuthSystem.Tg, cancellationToken)
            ?? throw new InvalidOperationException("User account not found");

        var history = await _duelHistoryHandler.GetDuelHistoryAsync(userAccount.Id, cancellationToken);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Profile, cancellationToken);
        if (scenario == null)
            return CommandResult.Error("Сценарий не найден");

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 3, cancellationToken);
        if (step == null)
            return CommandResult.Error("Шаг истории не найден");

        var message = await _messageFormatter.FormatDuelHistoryAsync(step.Message, history, cancellationToken);

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
