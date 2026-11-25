using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.System.Settings.Handlers;
using Mint.App.Services.UserInteractive.Duels.Dto;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Settings;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class VoteButtonHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    IDuelRepository duelRepository,
    IVoteRepository voteRepository,
    IAccountRepository accountRepository,
    IMessageFormatter messageFormatter) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    private readonly IDuelRepository _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));
    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));
    private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);
        ArgumentNullException.ThrowIfNull(updateCommand);

        var parts = callbackData.Split('_');
        if (parts.Length != 3 || parts[0] != ActionConstants.VotePrefix.TrimEnd('_'))
            return CommandResult.Error("Неверный формат");

        if (!long.TryParse(parts[1], out var duelId) || !long.TryParse(parts[2], out var optionId))
            return CommandResult.Error("Неверные данные");

        var hasVoted = await _voteRepository.HasUserVotedInDuelAsync(externalUserId, duelId, cancellationToken);
        if (hasVoted)
            return CommandResult.Error("Вы уже сделали ставку в этой дуэли");

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
            return CommandResult.Error("Сценарий не найден");

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 3, cancellationToken);
        if (step == null)
            return CommandResult.Error("Шаг ставки не найден");

        var balance = await _accountRepository.GetUserBalanceAsync(externalUserId, cancellationToken);
        var option = await _duelRepository.GetOptionByIdAsync(optionId, cancellationToken);

        var betData = new BetDataDto
        {
            DuelId = duelId,
            OptionId = optionId,
            SelectedOption = option?.OptionText ?? "Неизвестно",
            Balance = balance
        };

        var message = await _messageFormatter.FormatBetAsync(step.Message, betData, cancellationToken);

        var betButtons = new List<ButtonDto>
        {
            new() { Caption = "🪙 100", Action = $"{ActionConstants.BetPrefix}{duelId}_{optionId}_100" },
            new() { Caption = "🪙 500", Action = $"{ActionConstants.BetPrefix}{duelId}_{optionId}_500" },
            new() { Caption = "🪙 1000", Action = $"{ActionConstants.BetPrefix}{duelId}_{optionId}_1000" },
            new() { Caption = "❌ Отмена", Action = $"{ActionConstants.CancelPrefix}{duelId}" }
        };

        await _sessionRepository.CreateOrUpdateSessionAsync(
            externalUserId,
            scenario.Id,
            step.Id,
            $"{{\"step\":\"bet\",\"duel_id\":{duelId},\"option_id\":{optionId},\"message_id\":{updateCommand.MessageId}}}",
            cancellationToken);

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(betButtons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
