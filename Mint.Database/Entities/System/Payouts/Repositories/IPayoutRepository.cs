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

    /// <summary>
    /// Atomically claims a batch of pending payout plans for execution by moving them to
    /// <see cref="Mint.Common.Contracts.UserInteractive.Payouts.PayoutStatus.Processing"/>. Concurrent executors never receive the same payout.
    /// </summary>
    /// <param name="limit">Maximum number of payouts to claim.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Claimed payouts in the order they were created.</returns>
    Task<List<PayoutDto>> ClaimPendingPayoutsAsync(int limit, CancellationToken ct);

    /// <summary>
    /// Completes a claimed payout by attaching the transaction it produced.
    /// The update is applied only while the payout is still in <see cref="Mint.Common.Contracts.UserInteractive.Payouts.PayoutStatus.Processing"/>.
    /// </summary>
    /// <param name="id">Payout identifier.</param>
    /// <param name="transactionId">Identifier of the created transaction.</param>
    /// <param name="processedAt">Execution timestamp.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True when the payout was completed by this call.</returns>
    Task<bool> TryCompleteAsync(long id, long? transactionId, DateTimeOffset processedAt, CancellationToken ct);
}
