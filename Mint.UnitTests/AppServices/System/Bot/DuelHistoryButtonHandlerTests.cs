using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Buttons;
using Mint.App.Services.System.Bot.Handlers.Commands.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.App.Services.UserInteractive.Duels.Handlers;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Settings;
using Mint.Common.Contracts.UserInteractive.Duels;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.Ledger.Accounts.Dto;
using Moq;

namespace Mint.UnitTests.AppServices.System.Bot;

/// <summary>
/// Tests for <see cref="DuelHistoryButtonHandler"/>.
/// </summary>
public class DuelHistoryButtonHandlerTests
{
    private readonly Mock<IScenarioRepository> _scenarioRepository;
    private readonly Mock<IAccountRepository> _accountRepository;
    private readonly Mock<IDuelHistoryHandler> _duelHistoryHandler;
    private readonly Mock<IMessageFormatter> _messageFormatter;
    private readonly DuelHistoryButtonHandler _handler;

    public DuelHistoryButtonHandlerTests()
    {
        _scenarioRepository = new Mock<IScenarioRepository>();
        _accountRepository = new Mock<IAccountRepository>();
        _duelHistoryHandler = new Mock<IDuelHistoryHandler>();
        _messageFormatter = new Mock<IMessageFormatter>();

        _handler = new DuelHistoryButtonHandler(
            _scenarioRepository.Object,
            _accountRepository.Object,
            _duelHistoryHandler.Object,
            _messageFormatter.Object);
    }

    [Fact]
    public async Task HandleAsync_NullCallbackData_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _handler.HandleAsync(1001, null!, new UpdateCommandDto(), CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_AccountNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        _accountRepository
            .Setup(r => r.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None))
            .ReturnsAsync((AccountDto?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.HandleAsync(1001, "duel_history", new UpdateCommandDto(), CancellationToken.None));
        Assert.Equal("User account not found", exception.Message);
    }

    [Fact]
    public async Task HandleAsync_ScenarioNotFound_ReturnsError()
    {
        // Arrange
        _accountRepository
            .Setup(r => r.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None))
            .ReturnsAsync(new AccountDto { Id = 1, UserId = 1001, Balance = 1000m });

        _scenarioRepository
            .Setup(r => r.GetScenarioByNameAsync("profile", CancellationToken.None))
            .ReturnsAsync((ScenarioDto?)null);

        // Act
        var result = await _handler.HandleAsync(1001, "duel_history", new UpdateCommandDto(), CancellationToken.None);

        // Assert
        Assert.Equal("❌ Сценарий не найден", result.Message);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
    }

    [Fact]
    public async Task HandleAsync_StepNotFound_ReturnsError()
    {
        // Arrange
        var scenario = new ScenarioDto { Id = 2, Name = "profile" };
        _accountRepository
            .Setup(r => r.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None))
            .ReturnsAsync(new AccountDto { Id = 1, UserId = 1001, Balance = 1000m });

        _scenarioRepository
            .Setup(r => r.GetScenarioByNameAsync("profile", CancellationToken.None))
            .ReturnsAsync(scenario);

        _scenarioRepository
            .Setup(r => r.GetStepByOrderAsync(2, 3, CancellationToken.None))
            .ReturnsAsync((StepDto?)null);

        // Act
        var result = await _handler.HandleAsync(1001, "duel_history", new UpdateCommandDto(), CancellationToken.None);

        // Assert
        Assert.Equal("❌ Шаг истории не найден", result.Message);
        Assert.True(result.IsFinal);
        Assert.True(result.IsNewMessage);
    }

    [Fact]
    public async Task HandleAsync_Success_ReturnsFormattedHistory()
    {
        // Arrange
        var scenario = new ScenarioDto { Id = 2, Name = "profile" };
        var step = new StepDto { Id = 11, Message = "Тестовый шаблон", IsFinal = false };
        var buttons = new List<ButtonDto> { new() { Id = 15, Caption = "🔙 Вернуться в профиль" } };
        var account = new AccountDto { Id = 1, UserId = 1001, Balance = 1000m };
        var history = new List<DuelHistoryDto>
        {
            new()
            {
                DuelId = 1,
                CategoryName = "Криптовалюта",
                Result = DuelVoteResultType.Win,
                BetAmount = 100m,
                PayoutAmount = 200m,
                VotedAt = DateTimeOffset.UtcNow
            }
        };

        _accountRepository
            .Setup(r => r.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None))
            .ReturnsAsync(account);

        _scenarioRepository
            .Setup(r => r.GetScenarioByNameAsync("profile", CancellationToken.None))
            .ReturnsAsync(scenario);

        _scenarioRepository
            .Setup(r => r.GetStepByOrderAsync(2, 3, CancellationToken.None))
            .ReturnsAsync(step);

        _scenarioRepository
            .Setup(r => r.GetButtonsByStepIdAsync(11, CancellationToken.None))
            .ReturnsAsync(buttons);

        _duelHistoryHandler
            .Setup(h => h.GetDuelHistoryAsync(1, CancellationToken.None))
            .ReturnsAsync(history);

        _messageFormatter
            .Setup(f => f.FormatDuelHistoryAsync("Тестовый шаблон", history, CancellationToken.None))
            .ReturnsAsync("Сформированная история");

        // Act
        var result = await _handler.HandleAsync(1001, "duel_history", new UpdateCommandDto(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Сформированная история", result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Single(result.Keyboard);
        Assert.Equal("🔙 Вернуться в профиль", result.Keyboard.First().Caption);
        Assert.False(result.IsFinal);
        Assert.False(result.IsNewMessage);
    }

    [Fact]
    public async Task HandleAsync_EmptyHistory_ReturnsEmptyHistoryMessage()
    {
        // Arrange
        var scenario = new ScenarioDto { Id = 2, Name = "profile" };
        var step = new StepDto { Id = 11, Message = "Тестовый шаблон", IsFinal = false };
        var buttons = new List<ButtonDto>();
        var account = new AccountDto { Id = 1, UserId = 1001, Balance = 1000m };

        _accountRepository
            .Setup(r => r.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None))
            .ReturnsAsync(account);

        _scenarioRepository
            .Setup(r => r.GetScenarioByNameAsync("profile", CancellationToken.None))
            .ReturnsAsync(scenario);

        _scenarioRepository
            .Setup(r => r.GetStepByOrderAsync(2, 3, CancellationToken.None))
            .ReturnsAsync(step);

        _scenarioRepository
            .Setup(r => r.GetButtonsByStepIdAsync(11, CancellationToken.None))
            .ReturnsAsync(buttons);

        _duelHistoryHandler
            .Setup(h => h.GetDuelHistoryAsync(1, CancellationToken.None))
            .ReturnsAsync(new List<DuelHistoryDto>());

        _messageFormatter
            .Setup(f => f.FormatDuelHistoryAsync("Тестовый шаблон", It.Is<IReadOnlyList<DuelHistoryDto>>(h => h.Count == 0), CancellationToken.None))
            .ReturnsAsync("Пустая история");

        // Act
        var result = await _handler.HandleAsync(1001, "duel_history", new UpdateCommandDto(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Пустая история", result.Message);
    }
}
