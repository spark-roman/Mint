using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.System.Payouts;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Mappers;

/// <summary>
/// Mapper for duel history DTO
/// </summary>
public class DbDuelHistoryMapper : IDbDuelHistoryMapper
{
    /// <inheritdoc/>
    public DuelHistoryDto Map(DuelHistoryProjectionDto vote, Dictionary<long, PayoutEntity> payouts)
    {
        ArgumentNullException.ThrowIfNull(vote);
        ArgumentNullException.ThrowIfNull(payouts);

        payouts.TryGetValue(vote.Id, out var payout);

        var result = payout != null ? MapBonusTypeToResult(payout.BonusType) : DuelVoteResultType.NoVote;
        var payoutAmount = payout != null ? payout.Amount : 0m;

        return new DuelHistoryDto
        {
            DuelId = vote.DuelId,
            CategoryName = vote.CategoryName,
            Result = result,
            BetAmount = vote.BetAmount,
            PayoutAmount = payoutAmount,
            VotedAt = vote.CreatedAt
        };
    }

    private static DuelVoteResultType MapBonusTypeToResult(BonusType bonusType) => bonusType switch
    {
        BonusType.Payout => DuelVoteResultType.Win,
        BonusType.Refund => DuelVoteResultType.Draw,
        BonusType.NoActivity => DuelVoteResultType.Loss,
        _ => DuelVoteResultType.NoVote
    };
}

