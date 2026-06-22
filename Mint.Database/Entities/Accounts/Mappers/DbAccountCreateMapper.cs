using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Accounts.Dto;

namespace Mint.Database.Entities.Accounts.Mappers;

/// <inheritdoc/>
public class DbAccountCreateMapper : IDbEntityMapper<AccountCreateDto, AccountEntity>
{
    /// <inheritdoc/>
    public AccountEntity Map(AccountCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new AccountEntity
        {
            UserId = entity.UserId,
            Balance = entity.Balance,
            CreatedAt = entity.CreatedAt,
            LastTransactionDate = null
        };
    }
}
