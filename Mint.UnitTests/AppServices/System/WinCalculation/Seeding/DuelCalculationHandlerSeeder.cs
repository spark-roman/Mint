using Mint.Common.Contracts.Ledger.Accounts;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Database;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.Duels;
using Mint.Database.Entities.UserInteractive.Votes;
using Mint.Database.Entities.Users;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Seeding;

/// <summary>
/// Seeder for duel calculation handler test data.
/// </summary>
public static class DuelCalculationHandlerSeeder
{
    /// <summary>
    /// Seeds the database with test data for duel calculation handler tests.
    /// </summary>
    /// <param name="context">Database context to seed.</param>
    public static void Seed(MintDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var now = DateTimeOffset.UtcNow;
        
        context.Accounts.Add(
            new AccountEntity
            {
                Id = 1,
                UserId = 1,
                Balance = 1000000000m,
                CreatedAt = now,
                LastTransactionDate = now,
                Status = AccountStatus.Active
            });
    }
}
