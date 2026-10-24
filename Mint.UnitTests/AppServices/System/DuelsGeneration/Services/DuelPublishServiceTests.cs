using Microsoft.Extensions.Logging;
using Mint.App.Services.System.DuelsGeneration.Handlers;
using Mint.App.Services.System.DuelsGeneration.Services;
using Moq;

namespace Mint.UnitTests.AppServices.System.DuelsGeneration.Services;

/// <summary>
/// Tests for <see cref="DuelPublishService"/>.
/// </summary>
public class DuelPublishServiceTests
{
    /// <summary>
    /// Verifies that <see cref="DuelPublishService.ProcessPlannedDuelsAsync"/>
    /// delegates to <see cref="IDuelGenerationHandler.PublishDuelsAsync"/> and returns the count.
    /// </summary>
    [Fact]
    public async Task ProcessPlannedDuelsAsync_DelegatesToHandlerAndReturnsCount()
    {
        // Arrange
        var expectedCount = 10;
        var handlerMock = new Mock<IDuelGenerationHandler>();
        handlerMock
            .Setup(h => h.PublishDuelsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        var loggerMock = new Mock<ILogger<DuelPublishService>>();
        var service = new DuelPublishService(handlerMock.Object, loggerMock.Object);

        // Act
        var result = await service.ProcessPlannedDuelsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(expectedCount, result);
        handlerMock.Verify(h => h.PublishDuelsAsync(It.IsAny<CancellationToken>()), Times.Once);
        handlerMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that exceptions from the handler are logged and rethrown.
    /// </summary>
    [Fact]
    public async Task ProcessPlannedDuelsAsync_ExceptionIsLoggedAndRethrown()
    {
        // Arrange
        var ex = new InvalidOperationException("Publish error");
        var handlerMock = new Mock<IDuelGenerationHandler>();
        handlerMock
            .Setup(h => h.PublishDuelsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        var loggerMock = new Mock<ILogger<DuelPublishService>>();
        var service = new DuelPublishService(handlerMock.Object, loggerMock.Object);

        // Act & Assert
        var caught = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ProcessPlannedDuelsAsync(CancellationToken.None));
        Assert.Same(ex, caught);
    }
}
