using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Transactions.Dto;

namespace Mint.Database.Entities.Transactions.Mappers;

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
            TransactionType = entity.TransactionType,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt
        };
    }
}
