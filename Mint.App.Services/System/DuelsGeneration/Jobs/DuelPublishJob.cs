using Microsoft.Extensions.Logging;
using Mint.App.Services.Infrastructure.Jobs;
using Mint.App.Services.System.DuelsGeneration.Services;

namespace Mint.App.Services.System.DuelsGeneration.Jobs;

/// <summary>
/// Hangfire job for publishing duels.
/// </summary>
public sealed class DuelPublishJob(IDuelPublishService publishService, ILogger<DuelPublishJob> logger)
    : HangfireJobBase(logger), IDuelPublishJob
{
    private readonly IDuelPublishService _publishService = publishService
        ?? throw new ArgumentNullException(nameof(publishService));

    /// <inheritdoc />
    public Task ExecuteAsync() =>
        ExecuteAsync(
            TimeSpan.FromMinutes(30),
            ct => _publishService.ProcessPlannedDuelsAsync(ct),
            "DuelPublishJob");
}
