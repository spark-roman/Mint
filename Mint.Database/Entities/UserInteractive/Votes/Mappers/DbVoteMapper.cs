using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.UserInteractive.Votes.Dto;

namespace Mint.Database.Entities.UserInteractive.Votes.Mappers;

/// <inheritdoc/>
public class DbVoteMapper : IDbEntityMapper<VoteEntity, VoteDto>
{
    /// <inheritdoc/>
    public VoteDto Map(VoteEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new VoteDto
        {
            DuelId = entity.DuelId,
            AccountId = entity.AccountId,
            OptionChosen = entity.OptionChosen,
            BetAmount = entity.BetAmount,
            CreatedAt = entity.CreatedAt
        };
    }
}
