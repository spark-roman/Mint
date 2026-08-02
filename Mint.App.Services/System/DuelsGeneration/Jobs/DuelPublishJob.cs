using Microsoft.Extensions.Logging;
using Mint.App.Services.System.DuelsGeneration.Services;

namespace Mint.App.Services.System.DuelsGeneration.Jobs;

/// <summary>
/// Hangfire job for publishing duels.
/// </summary>
public sealed class DuelPublishJob(IDuelPublishService publishService, ILogger<DuelPublishJob> logger)
{
    private readonly IDuelPublishService _publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
    
    private readonly ILogger<DuelPublishJob> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Executes the duel publish job.
    /// </summary>
    public async Task ExecuteAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(30));

        try
        {
            _logger.LogInformation("DuelPublishJob started at {Time}", DateTimeOffset.UtcNow);

            var count = await _publishService.ProcessPlannedDuelsAsync(cts.Token);

            _logger.LogInformation("DuelPublishJob completed. Processed {Count} duels.", count);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("DuelPublishJob was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DuelPublishJob failed");
            throw;
        }
    }
}
