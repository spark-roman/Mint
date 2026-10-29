namespace Mint.App.Services.System.DuelsGeneration.Jobs;

/// <summary>
/// Interface for duel publish job.
/// </summary>
public interface IDuelPublishJob
{
    /// <summary>
    /// Executes the duel publish job.
    /// </summary>
    Task ExecuteAsync();
}
