using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mint.App.Services.Infrastructure.Jobs;
using Mint.App.Services.System.DuelsGeneration.Handlers;
using Mint.App.Services.System.DuelsGeneration.Services;

namespace Mint.App.Services.System.DuelsGeneration.Jobs;

/// <summary>
/// Hangfire job for generating duels using GigaChat.
/// </summary>
public sealed class DuelGenerationJob(
    IServiceScopeFactory serviceScopeFactory,
    TimeProvider timeProvider,
    ILogger<DuelGenerationJob> logger) : HangfireJobBase(logger), IDuelGenerationJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    private readonly TimeProvider _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));

    /// <inheritdoc />
    public async Task ExecuteAsync()
    {
        await ExecuteAsync(
            TimeSpan.FromMinutes(30),
            async ct =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var duelGenerationService = scope.ServiceProvider.GetRequiredService<IDuelGenerationService>();
                var duelGenerationHandler = scope.ServiceProvider.GetRequiredService<IDuelGenerationHandler>();

                var tomorrow = _timeProvider.GetUtcNow().AddDays(1);
                var expiresAt = new DateTimeOffset(tomorrow.Year, tomorrow.Month, tomorrow.Day, 3, 30, 0, TimeSpan.Zero);

                var duels = await duelGenerationService.GenerateDuelsWithGigaChatFromNewsAsync(5, expiresAt, ct);

                if (duels.Count == 0)
                {
                    return 0;
                }

                await duelGenerationHandler.HandleAsync(duels, ct);

                return duels.Count;
            },
            "DuelGenerationJob");
    }
}
