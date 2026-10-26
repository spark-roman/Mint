using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database.Entities.System.Payouts;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.Database.Entities.UserInteractive.Stats.Mappers;

/// <summary>
/// Mapper for duel history DTO
/// </summary>
public interface IDbDuelHistoryMapper
{
    /// <summary>
    /// Map vote projection and payouts to duel history DTO
    /// </summary>
    /// <param name="vote">Vote projection</param>
    /// <param name="payouts">Payouts dictionary</param>
    /// <returns>Duel history DTO</returns>
    DuelHistoryDto Map(DuelHistoryProjectionDto vote, Dictionary<long, PayoutEntity> payouts);
}
