using Microsoft.Extensions.Logging;
using Mint.App.Services.System.DuelsGeneration.Jobs;
using Mint.App.Services.System.WinCalculation.Jobs;
using Moq;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Jobs;

/// <summary>
/// Tests for <see cref="DuelPayoutPipelineJob"/>.
/// </summary>
public class DuelPayoutPipelineJobTests
{
    /// <summary>
    /// Verifies that the pipeline executes all three jobs in order.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_ExecutesAllJobsInOrder()
    {
        // Arrange
        var order = new List<string>();

        var payoutJobMock = new Mock<IDuelPayoutJob>();
        payoutJobMock
            .Setup(j => j.ExecuteAsync())
            .Returns(() => { order.Add("DuelPayout"); return Task.CompletedTask; });

        var executionJobMock = new Mock<IPayoutExecutionJob>();
        executionJobMock
            .Setup(j => j.ExecuteAsync())
            .Returns(() => { order.Add("PayoutExecution"); return Task.CompletedTask; });

        var publishJobMock = new Mock<IDuelPublishJob>();
        publishJobMock
            .Setup(j => j.ExecuteAsync())
            .Returns(() => { order.Add("DuelPublish"); return Task.CompletedTask; });

        var loggerMock = new Mock<ILogger<DuelPayoutPipelineJob>>();
        var job = new DuelPayoutPipelineJob(
            payoutJobMock.Object,
            executionJobMock.Object,
            publishJobMock.Object,
            loggerMock.Object);

        // Act
        await job.ExecuteAsync();

        // Assert
        Assert.Equal(["DuelPayout", "PayoutExecution", "DuelPublish"], order);
    }

    /// <summary>
    /// Verifies that if the first job fails, the remaining jobs are not executed.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_FirstJobFails_SubsequentJobsNotExecuted()
    {
        // Arrange
        var order = new List<string>();

        var payoutJobMock = new Mock<IDuelPayoutJob>();
        payoutJobMock
            .Setup(j => j.ExecuteAsync())
            .ThrowsAsync(new InvalidOperationException("Payout failed"));

        var executionJobMock = new Mock<IPayoutExecutionJob>();
        executionJobMock
            .Setup(j => j.ExecuteAsync())
            .Returns(() => { order.Add("PayoutExecution"); return Task.CompletedTask; });

        var publishJobMock = new Mock<IDuelPublishJob>();
        publishJobMock
            .Setup(j => j.ExecuteAsync())
            .Returns(() => { order.Add("DuelPublish"); return Task.CompletedTask; });

        var loggerMock = new Mock<ILogger<DuelPayoutPipelineJob>>();
        var job = new DuelPayoutPipelineJob(
            payoutJobMock.Object,
            executionJobMock.Object,
            publishJobMock.Object,
            loggerMock.Object);

        // Act & Assert
        var caught = await Assert.ThrowsAsync<InvalidOperationException>(
            () => job.ExecuteAsync());
        Assert.Equal("Payout failed", caught.Message);
        Assert.Empty(order);
    }

    /// <summary>
    /// Verifies that if the second job fails, the third job is not executed.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_SecondJobFails_ThirdJobNotExecuted()
    {
        // Arrange
        var order = new List<string>();

        var payoutJobMock = new Mock<IDuelPayoutJob>();
        payoutJobMock
            .Setup(j => j.ExecuteAsync())
            .Returns(() => { order.Add("DuelPayout"); return Task.CompletedTask; });

        var executionJobMock = new Mock<IPayoutExecutionJob>();
        executionJobMock
            .Setup(j => j.ExecuteAsync())
            .ThrowsAsync(new InvalidOperationException("Execution failed"));

        var publishJobMock = new Mock<IDuelPublishJob>();
        publishJobMock
            .Setup(j => j.ExecuteAsync())
            .Returns(() => { order.Add("DuelPublish"); return Task.CompletedTask; });

        var loggerMock = new Mock<ILogger<DuelPayoutPipelineJob>>();
        var job = new DuelPayoutPipelineJob(
            payoutJobMock.Object,
            executionJobMock.Object,
            publishJobMock.Object,
            loggerMock.Object);

        // Act & Assert
        var caught = await Assert.ThrowsAsync<InvalidOperationException>(
            () => job.ExecuteAsync());
        Assert.Equal("Execution failed", caught.Message);
        Assert.Equal(["DuelPayout"], order);
    }
}
