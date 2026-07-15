using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Votes.Dto;

namespace Mint.Database.Entities.UserInteractive.Votes.Mappers;

/// <inheritdoc/>
public class DbVoteCreateMapper : IDbEntityMapper<VoteCreateDto, VoteEntity>
{
    /// <inheritdoc/>
    public VoteEntity Map(VoteCreateDto entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new VoteEntity
        {
            DuelId = entity.DuelId,
            AccountId = entity.AccountId,
            ChosenOptionId = entity.ChosenOptionId,
            BetAmount = entity.BetAmount,
            TransactionId = entity.TransactionId,
            CreatedAt = entity.CreatedAt
        };
    }
}
