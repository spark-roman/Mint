using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class ButtonClickHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    //IMessageFormatter messageFormatter,
    IUserProfilesHandler profilesHandler) : IButtonHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    //private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));
    private readonly IUserProfilesHandler _profilesHandler = profilesHandler ?? throw new ArgumentNullException(nameof(profilesHandler));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        if (callbackData == "main_menu")
        {
            return await NavigateToScenarioAsync(externalUserId, "start", cancellationToken);
        }

        if (callbackData == "profile")
        {
            return await NavigateToScenarioAsync(externalUserId, "profile", cancellationToken);
        }

        if (callbackData == "duels")
        {
            return await NavigateToScenarioAsync(externalUserId, "duels", cancellationToken);
        }

        if (callbackData == "referral")
        {
            return await NavigateToScenarioAsync(externalUserId, "referral", cancellationToken);
        }

        if (callbackData == "claim_bonus")
        {
            return await HandleClaimBonusAsync(externalUserId, AuthSystem.Tg, cancellationToken);
        }

        if (callbackData == "leaderboard")
        {
            return await HandleLeaderboardAsync(externalUserId);
        }

        if (callbackData.StartsWith("category_", StringComparison.InvariantCultureIgnoreCase))
        {
            return await HandleCategorySelectionAsync(externalUserId, callbackData.Replace("category_", "", StringComparison.InvariantCultureIgnoreCase));
        }
        
        return await HandleButtonNavigationAsync(externalUserId, callbackData, cancellationToken);
    }

    private async Task<CommandResult> NavigateToScenarioAsync(long externalUserId, string scenarioName, CancellationToken cancellationToken)
    {
        var scenario = await _scenarioRepository.GetScenarioByNameAsync(scenarioName, cancellationToken);
        if (scenario == null)
        {
            return new CommandResult
            {
                Message = $"❌ Сценарий '{scenarioName}' не найден",
                IsFinal = true
            };
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
        {
            return new CommandResult
            {
                Message = "❌ Первый шаг не найден",
                IsFinal = true
            };
        }

        await _sessionRepository.CreateOrUpdateSessionAsync(
            externalUserId,
            scenario.Id,
            step.Id,
            "{}",
            cancellationToken);

        //var message = await _messageFormatter.FormatAsync(step.Message, externalUserId, cancellationToken);
        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        return new CommandResult
        {
            Message = step.Message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }

    private async Task<CommandResult> HandleClaimBonusAsync(long externalUserId, AuthSystem authSystem, CancellationToken cancellationToken)
    {
        var success = await _profilesHandler.ClaimDailyBonusAsync(externalUserId, authSystem, cancellationToken);
        if (!success)
        {
            return new CommandResult
            {
                Message = "❌ Бонус пока недоступен. Попробуйте позже.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        return await NavigateToScenarioAsync(externalUserId, "profile", cancellationToken);
    }

    private static Task<CommandResult> HandleLeaderboardAsync(long externalUserId)
    {
        var message = "📈 **Таблица лидеров**\n\nФункция в разработке...";
        
        var buttons = new List<ButtonDto>
        {
            new() { Caption = "🔙 Назад в профиль", Action = "profile", OrderNum = 1 }
        };

        return Task.FromResult(new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = true,
            IsNewMessage = false
        });
    }

    private static Task<CommandResult> HandleCategorySelectionAsync(long externalUserId, string categoryCode)
    {
        // TODO: Реализовать показ дуэлей по категории
        var message = $"📂 Вы выбрали категорию: {categoryCode}\n\nФункция в разработке...";
        
        var buttons = new List<ButtonDto>
        {
            new() { Caption = "⬅️ Назад к категориям", Action = "duels", OrderNum = 1 }
        };

        return Task.FromResult(new CommandResult
        {
            Message = message,
            Keyboard = new Collection<ButtonDto>(buttons),
            IsFinal = false,
            IsNewMessage = false
        });
    }

    private async Task<CommandResult> HandleButtonNavigationAsync(long externalUserId, string callbackData, CancellationToken cancellationToken)
    {
        var button = await _scenarioRepository.GetButtonByActionAsync(callbackData, cancellationToken);
        if (button == null)
        {
            return new CommandResult
            {
                Message = "❌ Неизвестное действие",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var nextStep = await _scenarioRepository.GetNextStepByButtonIdAsync(button.Id, cancellationToken);
        if (nextStep == null)
        {
            return new CommandResult
            {
                Message = "❌ Следующий шаг не найден",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var session = await _sessionRepository.GetActiveSessionAsync(externalUserId, cancellationToken);
        if (session != null)
        {
            await _sessionRepository.UpdateCurrentStepAsync(session.Id, nextStep.Id, cancellationToken);
        }

        //var message = await _messageFormatter.FormatAsync(nextStep.Message, externalUserId, cancellationToken);
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
