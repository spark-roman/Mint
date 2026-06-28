using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Database.Entities.Ledger.Transactions.Dto;

namespace Mint.Database.Entities.Ledger.Transactions.Mappers;

/// <inheritdoc/>
public class DbTransactionMapper : IDbEntityMapper<TransactionEntity, TransactionDto>
{
    /// <inheritdoc/>
    public TransactionDto Map(TransactionEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new TransactionDto
        {
            Id = entity.Id,
            AccountId = entity.AccountId,
            Amount = entity.Amount,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt,
            BounusType = (BonusType)entity.BonusTypeId
        };
    }
}
