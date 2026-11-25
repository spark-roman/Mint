using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.App.Services.UserInteractive.Profiles.Handlers;
using Mint.Common.Contracts.Users;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class BonusButtonHandler(
    IUserProfilesHandler profilesHandler) : IButtonHandler
{
    private readonly IUserProfilesHandler _profilesHandler = profilesHandler ?? throw new ArgumentNullException(nameof(profilesHandler));

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        var success = await _profilesHandler.ClaimDailyBonusAsync(externalUserId, AuthSystem.Tg, cancellationToken);
        if (!success)
            return CommandResult.Error("Бонус пока недоступен. Попробуйте позже.");

        return new CommandResult { Message = "Бонус зачислен!", IsFinal = true };
    }
}
