using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Ledger.Transactions.Dto;
using Mint.Database.Entities.System.Payouts.Dto;

namespace Mint.App.Services.System.WinCalculation.Mappers;

/// <inheritdoc cref="IDtoMapper{TDto, TEntity}"/>
public class PayoutTransactionMapper(TimeProvider timeProvider) : IDtoMapper<PayoutDto, TransactionCreateDto>
{
    private readonly TimeProvider _timeProvider = timeProvider;

    /// <inheritdoc/>
    public TransactionCreateDto Map(PayoutDto dto, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(dto);

        return new TransactionCreateDto
        {
            DebitAccountId = dto.DebitAccountId,
            CreditAccountId = dto.CreditAccountId,
            Amount = dto.Amount,
            Description = dto.Description,
            BonusType = dto.BonusType,
            CreatedAt = _timeProvider.GetUtcNow()
        };
    }
}
