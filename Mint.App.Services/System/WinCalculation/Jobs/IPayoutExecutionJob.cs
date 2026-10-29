namespace Mint.App.Services.System.WinCalculation.Jobs;

/// <summary>
/// Interface for payout execution job.
/// </summary>
public interface IPayoutExecutionJob
{
    /// <summary>
    /// Executes the payout execution job.
    /// </summary>
    Task ExecuteAsync();
}
