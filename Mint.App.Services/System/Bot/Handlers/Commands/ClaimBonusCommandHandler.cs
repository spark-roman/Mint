using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.UserInteractive.Bonuses.Handlers;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class ClaimBonusCommandHandler(
    IBonusCalculationHandler bonusHandler,
    IAccountRepository accountRepository,
    IServiceProvider serviceProvider) : ICommandHandler
{
    private readonly IBonusCalculationHandler _bonusHandler = bonusHandler
        ?? throw new ArgumentNullException(nameof(bonusHandler));
    
    private readonly IAccountRepository _accountRepository = accountRepository
        ?? throw new ArgumentNullException(nameof(accountRepository));

    private readonly IServiceProvider _serviceProvider = serviceProvider
        ?? throw new ArgumentNullException(nameof(serviceProvider));

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
                IsFinal = true,
                IsNewMessage = true
            };
        }

        var result = await _bonusHandler.ApplyDailyBonusAsync(
            tgUser.Id,
            (byte)AuthSystem.Tg,
            account.Id,
            cancellationToken);

        if (!result.Success || result.AlreadyApplied)
        {
            return new CommandResult
            {
                Message = string.Empty,
                IsFinal = true,
                IsNewMessage = false,
                Notification = result.AlreadyApplied 
                    ? "ℹ️ Бонус уже получен сегодня" 
                    : $"❌ {result.Message}"
            };
        }

        var profileHandler = _serviceProvider.GetRequiredService<ProfileCommandHandler>();
        return await profileHandler.HandleAsync(tgUser, "refresh", cancellationToken);
    }
}