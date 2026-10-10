using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Payouts.Dto;

namespace Mint.Database.Entities.System.Payouts.Mappers;

/// <summary>
/// Mapper from PayoutCreateDto to PayoutEntity.
/// </summary>
public sealed class DbPayoutCreateMapper : IDbEntityMapper<PayoutCreateDto, PayoutEntity>
{
    /// <inheritdoc />
    public PayoutEntity Map(PayoutCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new PayoutEntity
        {
            VoteId = entity.VoteId,
            DuelId = entity.DuelId,
            CreditAccountId = entity.AccountId,
            Amount = entity.Amount,
            DebitAccountId = entity.DebitAccountId,
            BonusType = entity.BonusType,
            Description = entity.Description,
            ProcessedAt = entity.ProcessedAt,
            Status = entity.Status,
            TransactionId = entity.TransactionId
        };
    }
}
