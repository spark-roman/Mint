using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Duels.Dto;
using Mint.App.Services.UserInteractive.Duels.Handlers;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions.Dto;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class ButtonClickHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    ICategoryRepository categoryRepository,
    IDuelRepository duelRepository,
    IVoteRepository voteRepository,
    IAccountRepository accountRepository,
    IUserProfilesHandler profilesHandler,
    IMessageFormatter messageFormatter,
    IDuelHandler duelHandler) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    
    private readonly ICategoryRepository _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    
    private readonly IDuelRepository _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));
    
    private readonly IUserProfilesHandler _profilesHandler = profilesHandler ?? throw new ArgumentNullException(nameof(profilesHandler));

    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));
    
    private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    
    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

    private readonly IDuelHandler _duelHandler = duelHandler ?? throw new ArgumentNullException(nameof(duelHandler));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        // === Навигация по сценариям ===
        if (callbackData == ActionConstants.MainMenu)
            return await NavigateToScenarioAsync(externalUserId, ScenarioConstants.Start, false, cancellationToken);

        if (callbackData == ActionConstants.Profile)
            return await NavigateToScenarioAsync(externalUserId, ScenarioConstants.Profile, false, cancellationToken);

        if (callbackData == ActionConstants.Duels)
            return await NavigateToScenarioAsync(externalUserId, ScenarioConstants.Duels, true, cancellationToken);

        if (callbackData == ActionConstants.Referral)
            return await NavigateToScenarioAsync(externalUserId, ScenarioConstants.Referral, false, cancellationToken);

        if (callbackData == ActionConstants.Leaderboard)
            return await NavigateToScenarioAsync(externalUserId, ScenarioConstants.Leaderboard, false, cancellationToken);

        // === Бонусы ===
        if (callbackData == ActionConstants.ClaimBonus)
            return await HandleClaimBonusAsync(externalUserId, AuthSystem.Tg, cancellationToken);

        // === Выбор категории ===
        if (callbackData.StartsWith(ActionConstants.CategoryPrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            var categoryCode = callbackData[ActionConstants.CategoryPrefix.Length..];
            return await HandleCategorySelectionAsync(externalUserId, categoryCode, cancellationToken);
        }

        // === Обработка голосования (v_duelId_optionId) ===
        if (callbackData.StartsWith(ActionConstants.VotePrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            return await HandleVoteSelectionAsync(externalUserId, callbackData, cancellationToken);
        }

        // === Обработка ставки (bet_duelId_optionId_amount) ===
        if (callbackData.StartsWith(ActionConstants.BetPrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            return await HandleBetPlacementAsync(externalUserId, callbackData, cancellationToken);
        }

        // === Обработка отмены (cancel_duelId) ===
        if (callbackData.StartsWith(ActionConstants.CancelPrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            return await HandleCancelAsync(externalUserId, callbackData, cancellationToken);
        }

        // === Стандартная навигация по кнопке ===
        return await HandleButtonNavigationAsync(externalUserId, callbackData, cancellationToken);
    }

    /// <summary>
    /// Navigates to the first step of a scenario.
    /// </summary>
    private async Task<CommandResult> NavigateToScenarioAsync(
        long externalUserId,
        string scenarioName,
        bool isNewMessage,
        CancellationToken cancellationToken)
    {
        var scenario = await _scenarioRepository.GetScenarioByNameAsync(scenarioName, cancellationToken);
        if (scenario == null)
        {
            return CommandResult.Error($"Сценарий '{scenarioName}' не найден");
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
        {
            return CommandResult.Error("Первый шаг не найден");
        }

        await _sessionRepository.CreateOrUpdateSessionAsync(
            externalUserId,
            scenario.Id,
            step.Id,
            "{}",
            cancellationToken);

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        return new CommandResult
        {
            Message = step.Message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = step.IsFinal,
            IsNewMessage = isNewMessage
        };
    }

    /// <summary>
    /// Handles daily bonus claiming.
    /// </summary>
    private async Task<CommandResult> HandleClaimBonusAsync(long externalUserId, AuthSystem authSystem, CancellationToken cancellationToken)
    {
        var success = await _profilesHandler.ClaimDailyBonusAsync(externalUserId, authSystem, cancellationToken);
        if (!success)
        {
            return CommandResult.Error("Бонус пока недоступен. Попробуйте позже.");
        }

        return await NavigateToScenarioAsync(externalUserId, ScenarioConstants.Profile, false, cancellationToken);
    }

    /// <summary>
    /// Handles category selection and shows the first available duel.
    /// </summary>
    private async Task<CommandResult> HandleCategorySelectionAsync(
        long externalUserId,
        string categoryCode,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByCodeAsync(categoryCode, cancellationToken);
        if (category == null)
        {
            return CommandResult.Error("Категория не найдена");
        }

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
        {
            return CommandResult.Error("Сценарий не найден");
        }

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, cancellationToken);
        if (step == null)
        {
            return CommandResult.Error("Шаг дуэли не найден");
        }

        var duel = await _duelRepository.GetFirstAvailableDuelAsync(category.Id, cancellationToken);
        if (duel == null)
        {
            return new CommandResult
            {
                Message = "😕 В этой категории пока нет активных дуэлей.",
                Keyboard = new Collection<ButtonDto>(
                [
                    new() { Caption = "⬅️ Назад", Action = ActionConstants.Duels }
                ]),
                IsFinal = false,
                IsNewMessage = false
            };
        }

        var duelCard = new DuelCardDto
        {
            DuelId = duel.Id,
            CategoryName = category.Name,
            Question = duel.Question,
            Description = duel.Description,
            ExpiresAt = duel.ExpiresAt,
            Options = [..duel.Options.Select(o => new DuelOptionDto
            {
                Id = o.Id,
                OptionText = o.OptionText,
                OptionCode = o.OptionCode
            })]
        };

        var message = await _messageFormatter.FormatDuelAsync(step.Message, duelCard, cancellationToken);

        var optionButtons = duel.Options.Select(o => new ButtonDto
        {
            Caption = o.OptionText,
            Action = $"{ActionConstants.VotePrefix}{duel.Id}_{o.Id}",
            OrderNum = (short)duel.Options.ToList().IndexOf(o)
        }).ToList();

        optionButtons.Add(new ButtonDto
        {
            Caption = "🔗 Поспорить с другом",
            Action = $"{ActionConstants.SharePrefix}{duel.Id}",
            OrderNum = (short)optionButtons.Count
        });

        await _sessionRepository.CreateOrUpdateSessionAsync(
            externalUserId,
            scenario.Id,
            step.Id,
            $"{{\"step\":\"duel\",\"duel_id\":{duel.Id}}}",
            cancellationToken);

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(optionButtons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }

    /// <summary>
    /// Handles vote selection and shows the bet screen.
    /// </summary>
    private async Task<CommandResult> HandleVoteSelectionAsync(
        long externalUserId,
        string callbackData,
        CancellationToken cancellationToken)
    {
        var parts = callbackData.Split('_');
        if (parts.Length != 3 || parts[0] != ActionConstants.VotePrefix.TrimEnd('_'))
        {
            return CommandResult.Error("Неверный формат");
        }

        if (!long.TryParse(parts[1], out var duelId) || !long.TryParse(parts[2], out var optionId))
        {
            return CommandResult.Error("Неверные данные");
        }
        
        var hasVoted = await _voteRepository.HasUserVotedInDuelAsync(externalUserId, duelId, cancellationToken);
        if (hasVoted)
        {
            return CommandResult.Error("Вы уже сделали ставку в этой дуэли");
        }

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
        {
            return CommandResult.Error("Сценарий не найден");
        }

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 3, cancellationToken);
        if (step == null)
        {
            return CommandResult.Error("Шаг ставки не найден");
        }
        
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
            new() { Caption = "💰 ВСЁ", Action = $"{ActionConstants.BetPrefix}{duelId}_{optionId}_{balance}" },
            new() { Caption = "❌ Отмена", Action = $"{ActionConstants.CancelPrefix}{duelId}" }
        };

        await _sessionRepository.CreateOrUpdateSessionAsync(
            externalUserId,
            scenario.Id,
            step.Id,
            $"{{\"step\":\"bet\",\"duel_id\":{duelId},\"option_id\":{optionId}}}",
            cancellationToken);

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(betButtons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }

    /// <summary>
    /// Handles bet placement and shows success screen.
    /// </summary>
    private async Task<CommandResult> HandleBetPlacementAsync(
        long externalUserId,
        string callbackData,
        CancellationToken cancellationToken)
    {
        var parts = callbackData.Split('_');
        if (parts.Length != 4 || parts[0] != ActionConstants.BetPrefix.TrimEnd('_'))
        {
            return CommandResult.Error("Неверный формат");
        }

        if (!long.TryParse(parts[1], out var duelId) ||
            !long.TryParse(parts[2], out var optionId) ||
            !decimal.TryParse(parts[3], out var amount))
        {
            return CommandResult.Error("Неверные данные");
        }
        
        var result = await _duelHandler.PlaceBetAsync(
            externalUserId,
            duelId,
            optionId,
            amount,
            cancellationToken);

        if (!result.Success)
        {
            return CommandResult.Error(result.Message);
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
            new() { Caption = "📊 Следующая дуэль", Action = ActionConstants.Duels }
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

    /// <summary>
    /// Handles cancel action and returns to duel selection.
    /// </summary>
    private async Task<CommandResult> HandleCancelAsync(
        long externalUserId,
        string callbackData,
        CancellationToken cancellationToken)
    {
        var parts = callbackData.Split('_');
        if (parts.Length != 2 || parts[0] != ActionConstants.CancelPrefix.TrimEnd('_'))
        {
            return CommandResult.Error("Неверный формат");
        }

        if (!long.TryParse(parts[1], out var duelId))
        {
            return CommandResult.Error("Неверные данные");
        }

        var session = await _sessionRepository.GetActiveSessionAsync(externalUserId, cancellationToken);
        if (session == null)
        {
            return CommandResult.Error("Сессия не найдена");
        }

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Duels, cancellationToken);
        if (scenario == null)
        {
            return CommandResult.Error("Сценарий не найден");
        }

        var step = await _scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, cancellationToken);
        if (step == null)
        {
            return CommandResult.Error("Шаг дуэли не найден");
        }

        await _sessionRepository.UpdateCurrentStepAsync(session.Id, step.Id, cancellationToken);

        var duel = await _duelRepository.GetDuelByIdAsync(duelId, cancellationToken);
        if (duel == null)
        {
            return CommandResult.Error("Дуэль не найдена");
        }

        var category = await _categoryRepository.GetByIdAsync(duel.CategoryId, cancellationToken);
        var duelCard = new DuelCardDto
        {
            DuelId = duel.Id,
            CategoryName = category?.Name ?? "Неизвестно",
            Question = duel.Question,
            Description = duel.Description,
            ExpiresAt = duel.ExpiresAt,
            Options = [..duel.Options.Select(o => new DuelOptionDto
            {
                Id = o.Id,
                OptionText = o.OptionText,
                OptionCode = o.OptionCode
            })]
        };

        var message = await _messageFormatter.FormatDuelAsync(step.Message, duelCard, cancellationToken);

        var optionButtons = duel.Options.Select(o => new ButtonDto
        {
            Caption = o.OptionText,
            Action = $"{ActionConstants.VotePrefix}{duel.Id}_{o.Id}",
            OrderNum = (short)duel.Options.ToList().IndexOf(o)
        }).ToList();

        optionButtons.Add(new ButtonDto
        {
            Caption = "🔗 Поспорить с другом",
            Action = $"{ActionConstants.SharePrefix}{duel.Id}",
            OrderNum = (short)optionButtons.Count
        });

        return new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(optionButtons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }

    /// <summary>
    /// Handles standard button navigation.
    /// </summary>
    private async Task<CommandResult> HandleButtonNavigationAsync(
        long externalUserId,
        string callbackData,
        CancellationToken cancellationToken)
    {
        var button = await _scenarioRepository.GetButtonByActionAsync(callbackData, cancellationToken);
        if (button == null)
        {
            return CommandResult.Error("Неизвестное действие");
        }

        var nextStep = await _scenarioRepository.GetNextStepByButtonIdAsync(button.Id, cancellationToken);
        if (nextStep == null)
        {
            return CommandResult.Error("Следующий шаг не найден");
        }

        var session = await _sessionRepository.GetActiveSessionAsync(externalUserId, cancellationToken);
        if (session != null)
        {
            await _sessionRepository.UpdateCurrentStepAsync(session.Id, nextStep.Id, cancellationToken);
        }

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(nextStep.Id, cancellationToken);

        return new CommandResult
        {
            Message = nextStep.Message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = nextStep.IsFinal,
            IsNewMessage = false
        };
    }
}