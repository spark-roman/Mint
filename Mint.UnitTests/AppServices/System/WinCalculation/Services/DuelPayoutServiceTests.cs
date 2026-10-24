using Microsoft.Extensions.Logging;
using Mint.App.Services.System.WinCalculation.Handlers;
using Mint.App.Services.System.WinCalculation.Services;
using Moq;

namespace Mint.UnitTests.AppServices.System.WinCalculation.Services;

/// <summary>
/// Tests for <see cref="DuelPayoutService"/>.
/// </summary>
public class DuelPayoutServiceTests
{
    /// <summary>
    /// Verifies that <see cref="DuelPayoutService.ProcessExpiredDuelsPayoutsAsync"/>
    /// delegates to <see cref="IDuelSettlementHandler.PlanDuelPayoutsAsync"/> and returns the count.
    /// </summary>
    [Fact]
    public async Task ProcessExpiredDuelsPayoutsAsync_DelegatesToHandlerAndReturnsCount()
    {
        // Arrange
        var expectedCount = 5;
        var settlementHandlerMock = new Mock<IDuelSettlementHandler>();
        settlementHandlerMock
            .Setup(h => h.PlanDuelPayoutsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        var loggerMock = new Mock<ILogger<DuelPayoutService>>();
        var service = new DuelPayoutService(settlementHandlerMock.Object, loggerMock.Object);

        // Act
        var result = await service.ProcessExpiredDuelsPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(expectedCount, result);
        settlementHandlerMock.Verify(h => h.PlanDuelPayoutsAsync(It.IsAny<CancellationToken>()), Times.Once);
        settlementHandlerMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that <see cref="DuelPayoutService.ExecutePendingPayoutsAsync"/>
    /// delegates to <see cref="IDuelSettlementHandler.ExecutePendingPayoutsAsync"/> and returns the count.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_DelegatesToHandlerAndReturnsCount()
    {
        // Arrange
        var expectedCount = 3;
        var settlementHandlerMock = new Mock<IDuelSettlementHandler>();
        settlementHandlerMock
            .Setup(h => h.ExecutePendingPayoutsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedCount);

        var loggerMock = new Mock<ILogger<DuelPayoutService>>();
        var service = new DuelPayoutService(settlementHandlerMock.Object, loggerMock.Object);

        // Act
        var result = await service.ExecutePendingPayoutsAsync(CancellationToken.None);

        // Assert
        Assert.Equal(expectedCount, result);
        settlementHandlerMock.Verify(h => h.ExecutePendingPayoutsAsync(It.IsAny<CancellationToken>()), Times.Once);
        settlementHandlerMock.VerifyNoOtherCalls();
    }

    /// <summary>
    /// Verifies that exceptions from the handler are logged and rethrown.
    /// </summary>
    [Fact]
    public async Task ProcessExpiredDuelsPayoutsAsync_ExceptionIsLoggedAndRethrown()
    {
        // Arrange
        var ex = new InvalidOperationException("Handler error");
        var settlementHandlerMock = new Mock<IDuelSettlementHandler>();
        settlementHandlerMock
            .Setup(h => h.PlanDuelPayoutsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        var loggerMock = new Mock<ILogger<DuelPayoutService>>();
        var service = new DuelPayoutService(settlementHandlerMock.Object, loggerMock.Object);

        // Act & Assert
        var caught = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ProcessExpiredDuelsPayoutsAsync(CancellationToken.None));
        Assert.Same(ex, caught);
    }

    /// <summary>
    /// Verifies that exceptions from the handler during execution are logged and rethrown.
    /// </summary>
    [Fact]
    public async Task ExecutePendingPayoutsAsync_ExceptionIsLoggedAndRethrown()
    {
        // Arrange
        var ex = new InvalidOperationException("Execution error");
        var settlementHandlerMock = new Mock<IDuelSettlementHandler>();
        settlementHandlerMock
            .Setup(h => h.ExecutePendingPayoutsAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(ex);

        var loggerMock = new Mock<ILogger<DuelPayoutService>>();
        var service = new DuelPayoutService(settlementHandlerMock.Object, loggerMock.Object);

        // Act & Assert
        var caught = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.ExecutePendingPayoutsAsync(CancellationToken.None));
        Assert.Same(ex, caught);
    }
}
