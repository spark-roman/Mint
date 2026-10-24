using System.Collections.ObjectModel;
using System.Transactions;
using Microsoft.Extensions.Logging;
using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.Settings;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Common.Contracts.UserInteractive.Payouts;
using Mint.Database.Entities.Ledger.Transactions.Dto;
using Mint.Database.Entities.Ledger.Transactions.Repositories;
using Mint.Database.Entities.System.Payouts.Dto;
using Mint.Database.Entities.System.Payouts.Repositories;
using Mint.Database.Entities.UserInteractive.Stats.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Repositories;

namespace Mint.App.Services.System.WinCalculation.Handlers;

/// <summary>
/// Handles execution of pending payouts.
/// </summary>
public interface IPayoutExecutionHandler
{
    /// <summary>
    /// Executes pending payouts in batches.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Count of executed payouts</returns>
    Task<int> ExecutePendingPayoutsAsync(CancellationToken cancellationToken);
}
