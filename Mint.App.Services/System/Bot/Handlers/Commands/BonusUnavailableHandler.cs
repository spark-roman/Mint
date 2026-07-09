using Mint.App.Services.System.Bot.Dto;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc />
public class BonusUnavailableHandler : ICommandHandler
{
    /// <inheritdoc />
    public Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CommandResult
        {
            Message = string.Empty,
            IsFinal = true,
            IsNewMessage = false,
            Notification = "⏳ Бонус пока недоступен. Загляните позже!"
        });
    }
}
