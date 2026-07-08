using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.UserInteractive.Bonuses.Handlers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class ClaimBonusCommandHandler(
    IBonusCalculationHandler bonusHandler,
    IAccountRepository accountRepository) : ICommandHandler
{
    private readonly IBonusCalculationHandler _bonusHandler = bonusHandler
        ?? throw new ArgumentNullException(nameof(bonusHandler));
    
    private readonly IAccountRepository _accountRepository = accountRepository
        ?? throw new ArgumentNullException(nameof(accountRepository));

    /// <inheritdoc/>
    public async Task<CommandResult> HandleAsync(User tgUser, string inputData, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var account = await _accountRepository.GetAccountByExternalUserIdAsync(
            tgUser.Id,
            (byte)AuthSystem.Tg,
            cancellationToken);

        if (account is null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: не удалось найти ваш аккаунт",
                IsFinal = true
            };
        }

        var result = await _bonusHandler.ApplyDailyBonusAsync(
            tgUser.Id,
            (byte)AuthSystem.Tg,
            account.Id,
            cancellationToken);

        var newBalance = await _accountRepository.GetUserBalanceAsync(tgUser.Id, cancellationToken);

        string message;
        if (!result.Success)
        {
            message = $"❌ {result.Message}";
        }
        else if (result.AlreadyApplied)
        {
            message = $"ℹ️ {result.Message}";
        }
        else
        {
            message = $"{result.Message}\n\n💳 Новый баланс: {newBalance:N0} 🪙";
        }

        return new CommandResult
        {
            Message = message,
            Keyboard = [],
            IsFinal = false,
            IsNewMessage = false
        };
    }
}