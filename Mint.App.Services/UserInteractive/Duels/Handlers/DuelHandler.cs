using AdvApplication.Auth.Users;
using Mint.App.Services.UserInteractive.Duels.Dto;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions.Dto;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;

namespace Mint.App.Services.UserInteractive.Duels.Handlers;

/// <inheritdoc />
public class DuelHandler(
    IDuelRepository duelRepository,
    IAccountRepository accountRepository,
    IUserRepository userRepository,
    ITransactionRepository transactionRepository,
    IVoteRepository voteRepository,
    TimeProvider timeProvider) : IDuelHandler
{
    private readonly IDuelRepository _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));

    private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));

    private readonly IUserRepository _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

    private readonly ITransactionRepository _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));

    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc />
    public async Task<DuelCardDto?> GetFirstAvailableDuelAsync(int categoryId, CancellationToken cancellationToken)
    {
        var duel = await _duelRepository.GetFirstAvailableDuelAsync(categoryId, cancellationToken);
        if (duel == null)
            return null;

        return new DuelCardDto
        {
            DuelId = duel.Id,
            CategoryName = duel.CategoryName,
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
    }

    /// <inheritdoc />
    public async Task<bool> HasUserVotedInDuelAsync(long externalUserId, long duelId, CancellationToken cancellationToken)
    {
        return await _voteRepository.HasUserVotedInDuelAsync(externalUserId, duelId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BetResultDto> PlaceBetAsync(
        long externalUserId,
        long duelId,
        long optionId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        var duel = await _duelRepository.GetDuelByIdAsync(duelId, cancellationToken);
        if (duel == null)
        {
            return new BetResultDto { Success = false, Message = "Дуэль не найдена" };
        }

        if (duel.IsClosed || duel.ExpiresAt < _timeProvider.GetUtcNow())
        {
            return new BetResultDto { Success = false, Message = "Дуэль уже закрыта" };
        }

        var hasVoted = await _voteRepository.HasUserVotedInDuelAsync(externalUserId, duelId, cancellationToken);
        if (hasVoted)
        {
            return new BetResultDto { Success = false, Message = "Вы уже сделали ставку" };
        }
        
        var option = await _duelRepository.GetOptionByIdAsync(optionId, cancellationToken);
        if (option == null)
        {
            return new BetResultDto { Success = false, Message = "Вариант ответа не найден" };
        }

        var user = await _userRepository.GetUserAsync(externalUserId, (byte)AuthSystem.Tg, cancellationToken);
        if (user == null)
        {
            return new BetResultDto { Success = false, Message = "Пользователь не найден" };
        }

        var account = await _accountRepository.GetAccountByExternalUserIdAsync(
            externalUserId,
            (byte)AuthSystem.Tg,
            cancellationToken);

        if (account == null)
        {
            return new BetResultDto { Success = false, Message = "Аккаунт не найден" };
        }

        if (account.Balance < amount)
        {
            return new BetResultDto { Success = false, Message = $"Недостаточно средств. Баланс: {account.Balance:N0} 🪙" };
        }

        var transaction = new TransactionCreateDto
        {
            DebitAccountId = account.Id,
            CreditAccountId = 1,
            Amount = amount,
            Description = $"Ставка на дуэль #{duelId}",
            BonusType = BonusType.Bet,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        var betTransactionId = await _transactionRepository.CreateTransactionAsync(transaction, cancellationToken);

        var vote = new VoteCreateDto
        {
            DuelId = duelId,
            ChosenOptionId = optionId,
            AccountId = account.Id,
            BetAmount = amount,
            TransactionId = betTransactionId,
            CreatedAt = _timeProvider.GetUtcNow()
        };

        var voteId = await _voteRepository.CreateVoteAsync(vote, cancellationToken);

        return new BetResultDto
        {
            Success = true,
            Message = "Ставка успешно принята!",
            NewBalance = account.Balance,
            VoteId = voteId
        };
    }
}
