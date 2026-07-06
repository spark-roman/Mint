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
            DebitAccountId = entity.DebitAccountId,
            CreditAccountId = entity.CreditAccountId,
            Amount = entity.Amount,
            BonusTypeId = (int)entity.BonusType,
            Description = entity.Description,
            CreatedAt = entity.CreatedAt
        };
    }
}
