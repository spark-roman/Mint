using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Mint.App.Services.System.Settings.Handlers;
using Mint.App.Services.System.WinCalculation.Dto;
using Mint.App.Services.System.WinCalculation.WinCalculationRules;
using Mint.Common.Contracts.Settings;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Dto;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <inheritdoc cref="IDuelCalculationHandler"/>
public sealed class DuelCalculationHandler(
    IAccountRepository accountRepository,
    ReadOnlyCollection<IWinCalculationRule> winCalculationRules,
    ISystemSettingHandler systemSettingHandler,
    TimeProvider timeProvider) : IDuelCalculationHandler
{
    private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));

    private readonly ReadOnlyCollection<IWinCalculationRule> _winCalculationRules = winCalculationRules ?? throw new ArgumentNullException(nameof(winCalculationRules));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    private readonly ISystemSettingHandler _systemSettingHandler = systemSettingHandler ?? throw new ArgumentNullException(nameof(systemSettingHandler));

    /// <inheritdoc />
    public async Task<DuelResultDto> CalculateResultAsync(DuelDto duel, long? winningOptionId, ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(duel);

        if (votes is null || votes.Count == 0)
        {
            return new DuelResultDto
            {
                DuelId = duel.Id,
                DuelType = (int)duel.DuelType,
                WinningOptionId = winningOptionId,
                TotalPot = 0,
                HouseCut = 0,
                VoteResults = []
            };
        }

        var systemAccount = await _accountRepository.GetSystemAccountAsync(cancellationToken);

        if (systemAccount is null)
        {
            throw new InvalidOperationException("System account not found");
        }

        if (winningOptionId is null)
        {
            var drawVoteDtos = votes.Select(v => new DuelVoteResultDto
            {
                PayoutInstruction = new TransactionCreateDto
                {
                    DebitAccountId = systemAccount.Id,
                    CreditAccountId = v.AccountId,
                    Amount = v.BetAmount,
                    Description = $"Возврат за ничью:{duel.Id}",
                    BonusType = BonusType.Refund,
                    CreatedAt = _timeProvider.GetUtcNow()
                },
                VoteAccountId = v.AccountId,
                VoteId = v.VoteId
            });

            return new DuelResultDto
            {
                DuelId = duel.Id,
                DuelType = (int)duel.DuelType,
                WinningOptionId = winningOptionId,
                TotalPot = votes.Sum(v => v.BetAmount),
                HouseCut = 0,
                VoteResults = [..drawVoteDtos]
            };
        }

        var houseCutPercent = await _systemSettingHandler.GetDecimalAsync(SettingKeysConstants.HouseCommission, 0.05m, cancellationToken);
        var totalPot = votes.Sum(v => v.BetAmount);
        var houseCut = totalPot * houseCutPercent;
        var prizePool = totalPot - houseCut;

        var winningVotes = votes.Where(v => v.ChosenOptionId == winningOptionId).ToList();
        var winningTotal = winningVotes.Sum(v => v.BetAmount);
        var winFactor = winningTotal > 0 ? prizePool / winningTotal : 1;

        var debitAccountId = systemAccount.Id;

        var winningVoteDtos = winningVotes.Select(v => new DuelVoteResultDto
        {
            PayoutInstruction = new TransactionCreateDto
            {
                DebitAccountId = systemAccount.Id,
                CreditAccountId = v.AccountId,
                Amount = v.BetAmount * winFactor,
                Description = $"Выплата за дуэль:{duel.Id}",
                BonusType = BonusType.Bet,
                CreatedAt = _timeProvider.GetUtcNow()
            },
            VoteAccountId = v.AccountId,
            VoteId = v.VoteId
        });

        var losesVoteDtos = votes
            .Where(v => v.ChosenOptionId != winningOptionId)
            .Select(v => new DuelVoteResultDto
            {
                PayoutInstruction = null,
                VoteAccountId = v.AccountId
            });

        return new DuelResultDto
        {
            DuelId = duel.Id,
            DuelType = (int)duel.DuelType,
            WinningOptionId = winningOptionId,
            TotalPot = totalPot,
            HouseCut = houseCut,
            VoteResults = [..winningVoteDtos, ..losesVoteDtos]
        };
    }

    /// <inheritdoc />
    public async Task<long?> CalculateWinningOptionIdAsync(DuelType duelType, ReadOnlyCollection<VoteDto> votes, CancellationToken cancellationToken)
    {
        if (votes is null || votes.Count == 0)
        {
            return null;
        }

        var tasks = _winCalculationRules.Select(async rule => new
        {
            Rule = rule,
            IsMatched = await rule.IsMatchedAsync(duelType)
        });

        var results = await Task.WhenAll(tasks);
        var matchedRule = results
            .Where(x => x.IsMatched)
            .Select(x => x.Rule)
            .FirstOrDefault();

        var winningOptionId = matchedRule is null 
            ? null
            : await matchedRule.CalculateAsync(votes, cancellationToken);

        return winningOptionId;
    }
}

