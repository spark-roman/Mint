namespace Mint.App.Services.System.WinCalculation.Jobs;

/// <summary>
/// Interface for duel payout job.
/// </summary>
public interface IDuelPayoutJob
{
    /// <summary>
    /// Executes the duel payout job.
    /// </summary>
    Task ExecuteAsync();
}
