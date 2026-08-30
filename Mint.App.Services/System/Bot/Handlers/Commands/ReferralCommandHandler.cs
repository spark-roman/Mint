using System.Globalization;
using AdvApplication.Auth.Users;
using HashidsNet;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.System.Settings.Handlers;
using Mint.App.Services.UserInteractive.Referral.Dto;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Settings;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class ReferralCommandHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    IUserRepository userRepository,
    IUserStatsRepository userStatsRepository,
    IMessageFormatter messageFormatter,
    ISystemSettingHandler systemSettingHandler,
    IHashids hashids,
    IOptions<TelegramOptions> telegramOptions,
    ILogger<ReferralCommandHandler> logger) : ICommandHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository ?? throw new ArgumentNullException(nameof(scenarioRepository));
    
    private readonly IUserSessionRepository _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));

    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly IUserStatsRepository _userStatsRepository = userStatsRepository ?? throw new ArgumentNullException(nameof(userStatsRepository));
    
    private readonly IMessageFormatter _messageFormatter = messageFormatter ?? throw new ArgumentNullException(nameof(messageFormatter));

    private readonly ISystemSettingHandler _systemSettingHandler = systemSettingHandler ?? throw new ArgumentNullException(nameof(systemSettingHandler));

    private readonly IHashids _hashids = hashids ?? throw new ArgumentNullException(nameof(hashids));

    private readonly TelegramOptions _botConfiguration = telegramOptions.Value;

    private readonly ILogger<ReferralCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        _logger.LogDebug("Handling referral command for user {UserId}", tgUser.Id);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync(ScenarioConstants.Referral, cancellationToken);

        if (scenario == null)
        {
            _logger.LogError("Referral scenario not found");

            return new CommandResult
            {
                Message = "❌ Раздел рефералов временно недоступен.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);

        if (step == null)
        {
            _logger.LogError("First step for referral scenario not found");

            return new CommandResult
            {
                Message = "❌ Контент рефералов не найден.",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var user = await _userRepository.GetUserAsync(tgUser.Id, (byte)AuthSystem.Tg, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("User {UserId} not found in database", tgUser.Id);

            return new CommandResult
            {
                Message = "❌ Пользователь не найден. Попробуйте /start",
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var userStats = await _userStatsRepository.GetStatsByUserIdAsync(user.ExternalUserId, (byte)AuthSystem.Tg, cancellationToken);
        var referralCode = _hashids.EncodeLong(tgUser.Id);

        var referralAmount = await _systemSettingHandler.GetDecimalAsync(
            SettingKeysConstants.ReferralBonus,
            5000m,
            cancellationToken);

        _logger.LogInformation("TelegramOptions: {Options}, Value:{Value}", _botConfiguration, _botConfiguration.BotUsername);

        var referralData = new ReferralDataDto
        {
            ReferralCode = referralCode,
            ReferralCount = userStats?.ReferralCount ?? 0,
            ReferralAmount = referralAmount,
            BotUserName = _botConfiguration.BotUsername
        };

        var buttons = await _scenarioRepository.GetButtonsByStepIdAsync(step.Id, cancellationToken);

        var buttonMessage = await _messageFormatter.FormatReferralMessageAsync(
            buttons[0].Action,
            referralData,
            cancellationToken);

        buttons[0].Action = buttonMessage;

        await _sessionRepository.CreateOrUpdateSessionAsync(
            user.ExternalUserId,
            scenario.Id,
            step.Id,
            "{}",
            cancellationToken);

        var formattedMessage = await _messageFormatter.FormatReferralMessageAsync(
            step.Message,
            referralData,
            cancellationToken);

        return new CommandResult
        {
            Message = formattedMessage,
            Keyboard = [..buttons],
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
