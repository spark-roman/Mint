using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.System.Payouts.Dto;

namespace Mint.Database.Entities.System.Payouts.Mappers;

/// <summary>
/// Mapper from PayoutEntity to PayoutDto.
/// </summary>
public sealed class DbPayoutMapper : IDbEntityMapper<PayoutEntity, PayoutDto>
{
    /// <inheritdoc />
    public PayoutDto Map(PayoutEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new PayoutDto
        {
            Id = entity.Id,
            VoteId = entity.VoteId,
            DuelId = entity.DuelId,
            CreditAccountId = entity.CreditAccountId,
            Amount = entity.Amount,
            Status = entity.Status,
            DebitAccountId = entity.DebitAccountId,
            BonusType = entity.BonusType,
            Description = entity.Description,
            TransactionId = entity.TransactionId,
            ProcessedAt = entity.ProcessedAt,
            CreatedAt = entity.PlannedAt
        };
    }
}
