using System.Collections.ObjectModel;
using Mint.Database.Entities.System.Payouts.Dto;

namespace Mint.Database.Entities.System.Payouts.Repositories;

/// <summary>
/// Repository for managing payouts.
/// </summary>
public interface IPayoutRepository
{
    /// <summary>Creates a new payout.</summary>
    Task<PayoutDto> CreateAsync(PayoutCreateDto dto, CancellationToken ct);

    /// <summary>Creates multiple payouts in batch.</summary>
    Task<List<PayoutDto>> CreateManyAsync(ReadOnlyCollection<PayoutCreateDto> dtos, CancellationToken ct);

    /// <summary>Updates a payout status.</summary>
    Task<PayoutDto> UpdateAsync(PayoutUpdateDto dto, CancellationToken ct);

    /// <summary>Updates multiple payouts statuses in batch.</summary>
    Task<List<PayoutDto>> UpdateManyAsync(ReadOnlyCollection<PayoutUpdateDto> dtos, CancellationToken ct);

    /// <summary>Gets a payout by its identifier.</summary>
    Task<PayoutDto?> GetByIdAsync(long id, CancellationToken ct);

    /// <summary>Gets payouts by duel identifier.</summary>
    Task<List<PayoutDto>> GetByDuelIdAsync(long duelId, CancellationToken ct);

    /// <summary>Gets payouts by account identifier.</summary>
    Task<List<PayoutDto>> GetByAccountIdAsync(long accountId, CancellationToken ct);

    /// <summary>Gets pending payouts for processing.</summary>
    Task<List<PayoutDto>> GetPendingPayoutsAsync(int limit, CancellationToken ct);
}
