using Microsoft.Extensions.Logging;

namespace Mint.App.Services.Infrastructure.Jobs;

/// <summary>
/// Base class for Hangfire jobs with unified timeout and logging.
/// </summary>
public abstract class HangfireJobBase(ILogger logger)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Executes the job with timeout and structured logging.
    /// </summary>
    /// <param name="timeout">Maximum execution time before cancellation.</param>
    /// <param name="execute">Async function to execute.</param>
    /// <param name="jobName">Name of the job for logging.</param>
    public async Task ExecuteAsync(TimeSpan timeout, Func<CancellationToken, Task<int>> execute, string jobName)
    {
        ArgumentNullException.ThrowIfNull(execute);

        using var cts = new CancellationTokenSource(timeout);

        try
        {
            _logger.LogInformation("{JobName} started at {Time}", jobName, DateTimeOffset.UtcNow);

            var count = await execute(cts.Token);

            _logger.LogInformation("{JobName} completed. Processed {Count} items.", jobName, count);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("{JobName} was cancelled", jobName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{JobName} failed", jobName);
            throw;
        }
    }
}
