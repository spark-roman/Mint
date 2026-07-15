using System.Collections.ObjectModel;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.App.Services.UserInteractive.Users.Dto;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class ProfileCommandHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    IMessageFormatter messageFormatter,
    IUserProfilesHandler userProfilesHandler,
    IDtoMapper<User, ExternalUserDto> userMapper) : ICommandHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository
        ?? throw new ArgumentNullException(nameof(scenarioRepository));
    
    private readonly IUserSessionRepository _sessionRepository = sessionRepository
        ?? throw new ArgumentNullException(nameof(sessionRepository));
    
    private readonly IMessageFormatter _messageFormatter = messageFormatter
        ?? throw new ArgumentNullException(nameof(messageFormatter));

    private readonly IUserProfilesHandler _userProfilesHandler = userProfilesHandler
        ?? throw new ArgumentNullException(nameof(userProfilesHandler));

    private readonly IDtoMapper<User, ExternalUserDto> _userMapper = userMapper ?? throw new ArgumentNullException(nameof(userMapper));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync("profile", cancellationToken);
        if (scenario == null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: сценарий 'profile' не найден",
                IsFinal = true
            };
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: первый шаг не найден",
                IsFinal = true
            };
        }

        await _sessionRepository.CreateOrUpdateSessionAsync(
            tgUser.Id,
            scenario.Id,
            step.Id,
            "{}",
            cancellationToken);

        var externalUser = _userMapper.Map(tgUser);
        var userProfile = await _userProfilesHandler.GetProfileAsync(externalUser.ExternalUserId, AuthSystem.Tg, cancellationToken);

        if (userProfile is null)
        {
            throw new InvalidOperationException($"Profile not found: {externalUser.ExternalUserId}");
        }

        var message = await _messageFormatter.FormatProfileAsync(step.Message, userProfile, cancellationToken);

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        var bonusButtonIndex = buttons.FindIndex(b => b.Action == "claim_bonus");
        if (bonusButtonIndex != -1)
        {
            buttons[bonusButtonIndex] = userProfile.CanClaimDailyBonus
                ? new ButtonDto
                {
                    Caption = "🎁 Забрать бонус",
                    Action = "claim_bonus",
                    OrderNum = buttons[bonusButtonIndex].OrderNum
                }
                : new ButtonDto
                {
                    Caption = $"⏳ Бонус через {userProfile.TimeUntilBonus:hh\\:mm\\:ss}",
                    Action = "bonus_unavailable",
                    OrderNum = buttons[bonusButtonIndex].OrderNum
                };
        }

        var isNewMessage = inputData == "start" || inputData == "new_message";

        return new CommandResult
        {
            Message = message,
            Keyboard = [..buttons],
            IsFinal = step.IsFinal,
            IsNewMessage = isNewMessage
        };
    }
}
