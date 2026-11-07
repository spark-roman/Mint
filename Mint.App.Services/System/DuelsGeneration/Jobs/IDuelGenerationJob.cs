namespace Mint.App.Services.System.DuelsGeneration.Jobs;

/// <summary>
/// Interface for duel generation job.
/// </summary>
public interface IDuelGenerationJob
{
    /// <summary>
    /// Executes the duel generation job.
    /// </summary>
    Task ExecuteAsync();
}
