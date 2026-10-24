using Microsoft.Extensions.Logging;
using Mint.App.Services.System.WinCalculation.Services;
using Mint.App.Services.System.WinCalculation.Jobs;
using Moq;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Jobs;

/// <summary>
/// Tests for <see cref="PayoutExecutionJob"/>.
/// </summary>
public class PayoutExecutionJobTests
{
    /// <summary>
    /// Verifies that <see cref="PayoutExecutionJob.ExecuteAsync"/> calls the service method.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_CallsService()
    {
        // Arrange
        var payoutServiceMock = new Mock<IDuelPayoutService>();
        payoutServiceMock
            .Setup(s => s.ExecutePendingPayoutsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);

        var loggerMock = new Mock<ILogger<PayoutExecutionJob>>();
        var job = new PayoutExecutionJob(payoutServiceMock.Object, loggerMock.Object);

        // Act
        await job.ExecuteAsync();

        // Assert
        payoutServiceMock.Verify(s => s.ExecutePendingPayoutsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that exceptions from the service are logged and rethrown.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithException_LogsAndRethrows()
    {
        // Arrange
        var ex = new InvalidOperationException("Service error");
        var payoutServiceMock = new Mock<IDuelPayoutService>();
        payoutServiceMock
            .Setup(s => s.ExecutePendingPayoutsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        var loggerMock = new Mock<ILogger<PayoutExecutionJob>>();
        var job = new PayoutExecutionJob(payoutServiceMock.Object, loggerMock.Object);

        // Act & Assert
        var caught = await Assert.ThrowsAsync<InvalidOperationException>(
            () => job.ExecuteAsync());
        Assert.Same(ex, caught);
    }
}
