using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Ledger.Transactions.Dto;

namespace Mint.Database.Entities.Ledger.Transactions.Mappers;

/// <inheritdoc/>
public class DbTransactionCreateMapper : IDbEntityMapper<TransactionCreateDto, TransactionEntity>
{
    /// <inheritdoc/>
    public TransactionEntity Map(TransactionCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new TransactionEntity
        {
            DebetAccountId = entity.DebetAccountId,
            CreditAccountId = entity.CreditAccountId,
            Amount = entity.Amount,
            BounusTypeId = (int)entity.BounusType,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt
        };
    }
}
