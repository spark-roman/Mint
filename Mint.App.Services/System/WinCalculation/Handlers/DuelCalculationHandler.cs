using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using Mint.App.Services.System.WinCalculation.Dto;
using Mint.App.Services.System.WinCalculation.WinCalculationRules;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Transactions.Dto;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <inheritdoc cref="IDuelCalculationHandler"/>
public sealed class DuelCalculationHandler(
    IDuelRepository duelRepository,
    IVoteRepository voteRepository,
    IAccountRepository accountRepository,
    ReadOnlyCollection<IWinCalculationRule> winCalculationRules,
    TimeProvider timeProvider) : IDuelCalculationHandler
{
    private readonly IDuelRepository _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));

    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));

    private readonly IAccountRepository _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));

    private readonly ReadOnlyCollection<IWinCalculationRule> _winCalculationRules = winCalculationRules ?? throw new ArgumentNullException(nameof(winCalculationRules));

    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    private const decimal HouseCutPercent = 0.05m;

    /// <inheritdoc />
    public async Task<DuelResultDto> CalculateResultAsync(long duelId, long winningOptionId, CancellationToken cancellationToken)
    {
        var duel = await _duelRepository.GetDuelByIdAsync(duelId, cancellationToken);
        
        if (duel == null)
        {
            throw new InvalidOperationException($"Duel {duelId} not found");
        }

        if (duel.Status == DuelStatus.Closed)
        {
            throw new InvalidOperationException($"Duel {duelId} is already closed");
        }
        
        var votes = await _voteRepository.GetVotesByDuelIdAsync(duelId, cancellationToken);

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

        var totalPot = votes.Sum(v => v.BetAmount);
        var houseCut = totalPot * HouseCutPercent;
        var prizePool = totalPot - houseCut;

        var winningVotes = votes.Where(v => v.ChosenOptionId == winningOptionId).ToList();
        var winningTotal = winningVotes.Sum(v => v.BetAmount);
        var winFactor = winningTotal > 0 ? prizePool / winningTotal : 1;

        var systemAccount = await _accountRepository.GetSystemAccountAsync(cancellationToken);

        if (systemAccount is null)
        {
            throw new InvalidOperationException("System account not found");
        }

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
            VoteAccountId = v.AccountId
        }).ToList();

        var losesVotes  = votes.Where(v => v.ChosenOptionId != winningOptionId).ToList();
        var losesVoteDtos = losesVotes.Select(v => new DuelVoteResultDto
        {
            PayoutInstruction = null,
            VoteAccountId = v.AccountId
        }).ToList();

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
    public async Task<List<long>> CalculateWinningOptionIdAsync(long duelId, DuelType duelType, CancellationToken cancellationToken)
    {
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

        var winningOptionIds = matchedRule is null ? [] : await matchedRule.CalculateAsync(duelId, cancellationToken);

        return winningOptionIds;
    }
}

