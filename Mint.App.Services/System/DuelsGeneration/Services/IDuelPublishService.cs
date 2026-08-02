namespace Mint.App.Services.System.DuelsGeneration.Services;

/// <summary>
/// Service for publishing duels.
/// </summary>
public interface IDuelPublishService
{
    /// <summary>
    /// Processes all planned duels.
    /// </summary>
    Task<int> ProcessPlannedDuelsAsync(CancellationToken cancellationToken);
}
