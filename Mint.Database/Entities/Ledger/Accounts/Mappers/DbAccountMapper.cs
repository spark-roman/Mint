using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Ledger.Accounts.Dto;

namespace Mint.Database.Entities.Ledger.Accounts.Mappers;

/// <inheritdoc/>
public class DbAccountMapper : IDbEntityMapper<AccountEntity, AccountDto>
{
    /// <inheritdoc/>
    public AccountDto Map(AccountEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AccountDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Balance = entity.Balance,
            CreatedAt = entity.CreatedAt,
            LastTransactionDate = entity.LastTransactionDate,
            Status = entity.Status
        };
    }
}
