using Microsoft.Extensions.Logging;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.UserInteractive.Duels.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <inheritdoc cref="IDuelSettlementHandler"/>
public sealed class DuelSettlementHandler(
    IDuelRepository duelRepository,
    IVoteRepository voteRepository,
    ITransactionRepository transactionRepository,
    IDuelCalculationHandler duelCalculator,
    ILogger<DuelSettlementHandler> logger) : IDuelSettlementHandler
{
    private readonly IDuelRepository _duelRepository = duelRepository ?? throw new ArgumentNullException(nameof(duelRepository));

    private readonly IVoteRepository _voteRepository = voteRepository ?? throw new ArgumentNullException(nameof(voteRepository));

    private readonly ITransactionRepository _transactionRepository = transactionRepository
        ?? throw new ArgumentNullException(nameof(transactionRepository));

    private readonly IDuelCalculationHandler _duelCalculator = duelCalculator
        ?? throw new ArgumentNullException(nameof(duelCalculator));

    private readonly ILogger<DuelSettlementHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<int> SettleExpiredDuelsAsync(CancellationToken cancellationToken)
    {
        var expiredDuels = await _duelRepository.GetActiveDuelsAsync(cancellationToken);

        if (expiredDuels is null || expiredDuels.Count == 0)
        {
            _logger.LogInformation("No expired duels to settle");
            return 0;
        }

        _logger.LogInformation("Settling {Count} expired duels", expiredDuels.Count);

        var settledCount = 0;

        foreach (var duel in expiredDuels)
        {
            await SettleDuelByVotesAsync(duel.Id, cancellationToken);
            settledCount++;
        }

        return settledCount;
    }

    /// <inheritdoc />
    public async Task SettleDuelAsync(long duelId, CancellationToken cancellationToken)
    {
        var duel = await _duelRepository.GetDuelByIdAsync(duelId, cancellationToken);

        if (duel == null)
        {
            throw new InvalidOperationException($"Duel {duelId} not found");
        }

        if (duel.IsClosed)
        {
            throw new InvalidOperationException($"Duel {duelId} is already closed");
        }
        
        await SettleDuelByVotesAsync(duel.Id, cancellationToken);
    }

    private async Task SettleDuelByVotesAsync(long duelId, CancellationToken cancellationToken)
    {
        var votes = await _voteRepository.GetVotesByDuelIdAsync(duelId, cancellationToken);

        if (votes is null || votes.Count == 0)
        {
            _logger.LogWarning("No votes found for duel {DuelId}, closing without settlement", duelId);
            await _duelRepository.CloseDuelAsync(duelId, cancellationToken);
            return;
        }

        var winningOptionId = votes
            .GroupBy(v => v.ChosenOptionId)
            .OrderByDescending(g => g.Sum(v => v.BetAmount))
            .First()
            .Key;

        await ProcessSettlementAsync(duelId, winningOptionId, cancellationToken);

        _logger.LogInformation(
            "Duel {DuelId} settled with winning option {WinningOptionId} by majority vote",
            duelId,
            winningOptionId);
    }

    private async Task ProcessSettlementAsync(long duelId, long winningOptionId, CancellationToken cancellationToken)
    {
        var result = await _duelCalculator.CalculateResultAsync(duelId, winningOptionId, cancellationToken);

        if (result.PayoutInstructions.Count == 0)
        {
            _logger.LogWarning("No payout instructions for duel {DuelId}", duelId);
            await _duelRepository.CloseDuelAsync(duelId, cancellationToken);
            return;
        }

        foreach (var instruction in result.PayoutInstructions)
        {
            await _transactionRepository.CreateTransactionAsync(instruction, cancellationToken);

            _logger.LogDebug(
                "Payout: {Amount} to account {CreditAccountId}",
                instruction.Amount,
                instruction.CreditAccountId);
        }

        await _duelRepository.CloseDuelAsync(duelId, cancellationToken);

        _logger.LogInformation(
            "Duel {DuelId} settled: {PayoutCount} payouts, total {TotalPayout}",
            duelId,
            result.PayoutInstructions.Count,
            result.PayoutInstructions.Sum(i => i.Amount));
    }
}
