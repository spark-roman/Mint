using Microsoft.Extensions.Logging;
using Mint.App.Services.System.DuelsGeneration.Services;
using Mint.App.Services.System.DuelsGeneration.Jobs;
using Moq;

namespace Mint.UnitTests.System.DuelsGeneration.Jobs;

/// <summary>
/// Tests for <see cref="DuelPublishJob"/>.
/// </summary>
public class DuelPublishJobTests
{
    /// <summary>
    /// Verifies that <see cref="DuelPublishJob.ExecuteAsync"/> calls the service method.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_CallsService()
    {
        // Arrange
        var publishServiceMock = new Mock<IDuelPublishService>();
        publishServiceMock
            .Setup(s => s.ProcessPlannedDuelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        var loggerMock = new Mock<ILogger<DuelPublishJob>>();
        var job = new DuelPublishJob(publishServiceMock.Object, loggerMock.Object);

        // Act
        await job.ExecuteAsync();

        // Assert
        publishServiceMock.Verify(s => s.ProcessPlannedDuelsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Verifies that exceptions from the service are logged and rethrown.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithException_LogsAndRethrows()
    {
        // Arrange
        var ex = new InvalidOperationException("Service error");
        var publishServiceMock = new Mock<IDuelPublishService>();
        publishServiceMock
            .Setup(s => s.ProcessPlannedDuelsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        var loggerMock = new Mock<ILogger<DuelPublishJob>>();
        var job = new DuelPublishJob(publishServiceMock.Object, loggerMock.Object);

        // Act & Assert
        var caught = await Assert.ThrowsAsync<InvalidOperationException>(
            () => job.ExecuteAsync());
        Assert.Same(ex, caught);
    }
}
