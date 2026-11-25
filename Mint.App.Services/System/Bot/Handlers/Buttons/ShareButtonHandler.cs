using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;

namespace Mint.App.Services.System.Bot.Handlers.Buttons;

/// <inheritdoc cref="IButtonHandler"/>
public sealed class ShareButtonHandler : IButtonHandler
{
    /// <inheritdoc />
    public Task<CommandResult> HandleAsync(long externalUserId, string callbackData, UpdateCommandDto updateCommand, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(callbackData);

        // Share is handled by the bot itself (Telegram share link)
        // Return empty result to let the bot handle it naturally
        return Task.FromResult(new CommandResult { Message = "", IsFinal = true });
    }
}
