using Microsoft.Extensions.Logging;
using Mint.App.Services.System.WinCalculation.Services;
using Mint.App.Services.System.WinCalculation.Jobs;
using Moq;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Jobs;

/// <summary>
/// Tests for <see cref="DuelPayoutJob"/>.
/// </summary>
public class DuelPayoutJobTests
{
    /// <summary>
    /// Verifies that <see cref="DuelPayoutJob.ExecuteAsync"/> calls the service method.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_CallsService()
    {
        // Arrange
        var payoutServiceMock = new Mock<IDuelPayoutService>();
        payoutServiceMock
            .Setup(s => s.ProcessExpiredDuelsPayoutsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var loggerMock = new Mock<ILogger<DuelPayoutJob>>();
        var job = new DuelPayoutJob(payoutServiceMock.Object, loggerMock.Object);

        // Act
        await job.ExecuteAsync();

        // Assert
        payoutServiceMock.Verify(s => s.ProcessExpiredDuelsPayoutsAsync(It.IsAny<CancellationToken>()), Times.Once);
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
            .Setup(s => s.ProcessExpiredDuelsPayoutsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        var loggerMock = new Mock<ILogger<DuelPayoutJob>>();
        var job = new DuelPayoutJob(payoutServiceMock.Object, loggerMock.Object);

        // Act & Assert
        var caught = await Assert.ThrowsAsync<InvalidOperationException>(
            () => job.ExecuteAsync());
        Assert.Same(ex, caught);
    }
}
