using Microsoft.Extensions.Logging;
using Mint.App.Services.System.DuelsGeneration.Handlers;

namespace Mint.App.Services.System.DuelsGeneration.Services;

/// <inheritdoc cref="IDuelPublishService"/>
public sealed class DuelPublishService(
    IDuelGenerationHandler duelGenerationHandler,
    ILogger<DuelPublishService> logger) : IDuelPublishService
{
    private readonly IDuelGenerationHandler _duelGenerationHandler = duelGenerationHandler
       ?? throw new ArgumentNullException(nameof(duelGenerationHandler));
    
    private readonly ILogger<DuelPublishService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public async Task<int> ProcessPlannedDuelsAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting publishing duels at {Time}", DateTimeOffset.UtcNow);

            var settledCount = await _duelGenerationHandler.PublishDuelsAsync(cancellationToken);

            _logger.LogInformation(
                "Published {Count} duels at {Time}",
                settledCount,
                DateTimeOffset.UtcNow);

            return settledCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish duels");
            throw;
        }
    }
}

