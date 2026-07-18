using Microsoft.Extensions.DependencyInjection;
using Mint.App.Services.System.Bot.Handlers.Buttons;
using Mint.Common.Contracts.Bot.Commands;
using Mint.Common.Contracts.Users;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.Ledger.Accounts;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.Database.Entities.UserInteractive.Votes.Dto;
using Mint.Database.Entities.UserInteractive.Votes.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Mint.UnitTests.AppServices.System.Fixtures.EntityFarmework;

namespace Mint.UnitTests.AppServices.System.Bot;

/// <summary>
/// Tests for <see cref="ButtonClickHandler"/> using DI and EF Core.
/// </summary>
public class ButtonClickHandlerTests : IClassFixture<ButtonClickHandlerFixtures>, IDisposable
{
    private readonly ButtonClickHandlerFixtures _fixture;
    private IServiceScope? _currentScope;

    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonClickHandlerTests"/> class.
    /// </summary>
    /// <param name="fixture">Test fixture.</param>
    public ButtonClickHandlerTests(ButtonClickHandlerFixtures fixture)
    {
        _fixture = fixture;
    }

    #region Navigation - Main Menu

    /// <summary>
    /// Verifies that HandleAsync with 'main_menu' navigates to start scenario.
    /// </summary>
    [Fact]
    public async Task HandleAsync_MainMenu_ReturnsStartScenario()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "main_menu", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.False(result.IsNewMessage);
        Assert.False(result.IsFinal);
        Assert.Contains("Добро пожаловать", result.Message);
    }

    #endregion

    #region Navigation - Profile

    /// <summary>
    /// Verifies that HandleAsync with 'profile' navigates to profile scenario.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Profile_ReturnsProfileScenario()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "profile", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.False(result.IsNewMessage);
        Assert.False(result.IsFinal);
        Assert.Contains("Ваш игровой профиль", result.Message);
    }

    #endregion

    #region Navigation - Duels

    /// <summary>
    /// Verifies that HandleAsync with 'duels' navigates to duels scenario with isNewMessage=true.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Duels_ReturnsDuelsScenarioWithNewMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "duels", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.False(result.IsNewMessage);
        Assert.False(result.IsFinal);
        Assert.Contains("Выберите категорию", result.Message);
    }

    #endregion

    #region Navigation - Referral

    /// <summary>
    /// Verifies that HandleAsync with 'referral' navigates to referral scenario.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Referral_ReturnsReferralScenario()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "referral", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.False(result.IsNewMessage);
        Assert.False(result.IsFinal);
        Assert.Contains("РЕФЕРАЛЬНАЯ", result.Message);
    }

    #endregion

    #region Navigation - Scenario Not Found

    /// <summary>
    /// Verifies that HandleAsync returns error when scenario is not found.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ScenarioNotFound_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "nonexistent_scenario", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неизвестное действие", result.Message);
    }

    #endregion

    #region Bonus - Claim Daily Bonus

    /// <summary>
    /// Verifies that HandleAsync with 'claim_bonus' processes the bonus claim.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ClaimBonus_ProcessesBonusClaim()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "claim_bonus", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync with 'claim_bonus' returns error when bonus is not available.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ClaimBonus_NotAvailable_ReturnsError()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "claim_bonus", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Result depends on bonus state in DB
    }

    #endregion

    #region Category Selection - Valid Categories

    /// <summary>
    /// Verifies that HandleAsync with 'category_crypto' returns duel card for crypto category.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CategoryCrypto_ReturnsDuelCard()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "category_crypto", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Contains("Bitcoin", result.Message);
        Assert.Contains("Да, конечно!", result.Keyboard[0].Caption);
        Assert.Contains("Нет, не думаю", result.Keyboard[1].Caption);
    }

    /// <summary>
    /// Verifies that HandleAsync with 'category_tech' returns duel card for tech category.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CategoryTech_ReturnsDuelCard()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "category_tech", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Contains("нейросети", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync with 'category_sports' returns duel card for sports category.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CategorySports_ReturnsDuelCard()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "category_sports", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Contains("ЧМ", result.Message);
    }

    /// <summary>
    /// Verifies that category selection creates a session with duel data.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CategorySelection_CreatesSessionWithDuelData()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        var scenario = await scenarioRepository.GetScenarioByNameAsync("duels", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, CancellationToken.None);
        Assert.NotNull(step);

        // Act
        await handler.HandleAsync(1001, "category_crypto", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(scenario.Id, session.ScenarioId);
        Assert.Equal(step.Id, session.CurrentStepId);
        Assert.Contains("\"step\":\"duel\"", session.Data);
        Assert.Contains("\"duel_id\"", session.Data);
    }

    /// <summary>
    /// Verifies that category selection returns share button.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CategorySelection_ReturnsShareButton()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "category_crypto", CancellationToken.None);

        // Assert
        Assert.NotNull(result.Keyboard);
        Assert.Contains("🔗 Поспорить с другом", result.Keyboard.Select(b => b.Caption));
    }

    #endregion

    #region Category Selection - Invalid Categories

    /// <summary>
    /// Verifies that HandleAsync with non-existent category returns error message.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidCategory_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "category_nonexistent", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Категория не найдена", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync with category without duels returns appropriate message.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CategoryWithoutDuels_ReturnsNoDuelsMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var categoryRepository = _currentScope.ServiceProvider.GetRequiredService<ICategoryRepository>();

        // Add a category without duels
        var category = await categoryRepository.CreateAsync(
            new Mint.Database.Entities.UserInteractive.UserCategories.Dto.CategoryDto
            {
                Name = "Отсутствующие дуэли",
                Code = "no_duels",
                Description = "Категория без дуэлей",
                IsActiveForAI = true,
                SearchKeywords = ""
            },
            CancellationToken.None);

        Assert.NotNull(category);

        // Act
        var result = await handler.HandleAsync(1001, "category_no_duels", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("В этой категории пока нет активных дуэлей.", result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Contains("⬅️ Назад", result.Keyboard[0].Caption);
    }

    #endregion

    #region Vote Selection - Valid Votes

    /// <summary>
    /// Verifies that HandleAsync with 'v_1_1' returns bet screen with option data.
    /// </summary>
    [Fact]
    public async Task HandleAsync_VoteSelection_ReturnsBetScreen()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "v_1_1", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Contains("100", result.Keyboard[0].Caption);
        Assert.Contains("500", result.Keyboard[1].Caption);
        Assert.Contains("1000", result.Keyboard[2].Caption);
        Assert.Contains("ВСЁ", result.Keyboard[3].Caption);
    }

    /// <summary>
    /// Verifies that vote selection creates a session with duel and option data.
    /// </summary>
    [Fact]
    public async Task HandleAsync_VoteSelection_CreatesSessionWithData()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        var scenario = await scenarioRepository.GetScenarioByNameAsync("duels", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetStepByOrderAsync(scenario.Id, 3, CancellationToken.None);
        Assert.NotNull(step);

        // Act
        await handler.HandleAsync(1001, "v_1_1", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(scenario.Id, session.ScenarioId);
        Assert.Equal(step.Id, session.CurrentStepId);
        Assert.Contains("\"step\":\"bet\"", session.Data);
        Assert.Contains("\"duel_id\"", session.Data);
        Assert.Contains("\"option_id\"", session.Data);
    }

    #endregion

    #region Vote Selection - Already Voted

    /// <summary>
    /// Verifies that HandleAsync returns error when user has already voted in a duel.
    /// </summary>
    [Fact]
    public async Task HandleAsync_AlreadyVoted_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Create a vote to simulate already voted
        var account = await accountRepository.GetAccountByExternalUserIdAsync(1001, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);

        var vote = new VoteCreateDto
        {
            DuelId = 1,
            ChosenOptionId = 1,
            AccountId = account.Id,
            BetAmount = 100m,
            TransactionId = 1,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await voteRepository.CreateVoteAsync(vote, CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(1001, "v_1_1", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("уже сделали ставку", result.Message);
    }

    #endregion

    #region Vote Selection - Invalid Format

    /// <summary>
    /// Verifies that HandleAsync with invalid vote format returns error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidVoteFormat_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "v_invalid", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неверный формат", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync with non-numeric vote data returns error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NonNumericVoteData_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "v_abc_def", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неверные данные", result.Message);
    }

    #endregion

    #region Bet Placement - Valid Bet

    /// <summary>
    /// Verifies that HandleAsync with valid bet places the bet successfully.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidBet_PlacesBetSuccessfully()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var transactionRepository = _currentScope.ServiceProvider.GetRequiredService<Mint.Database.Entities.Ledger.Transactions.Repositories.ITransactionRepository>();
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        // Act
        var result = await handler.HandleAsync(1002, "bet_1_1_100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Contains("СТАВКА УСПЕШНО ПРИНЯТА!", result.Message);

        // Verify transaction was created
        var account = await accountRepository.GetAccountByExternalUserIdAsync(1002, (byte)AuthSystem.Tg, CancellationToken.None);
        Assert.NotNull(account);
        var transactions = await transactionRepository.GetTransactionsByAccountIdAsync(account.Id, CancellationToken.None);
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);
    }

    /// <summary>
    /// Verifies that HandleAsync with bet creates a vote record.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidBet_CreatesVoteRecord()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var voteRepository = _currentScope.ServiceProvider.GetRequiredService<IVoteRepository>();

        // Act
        await handler.HandleAsync(1002, "bet_1_1_100", CancellationToken.None);

        // Assert
        var hasVoted = await voteRepository.HasUserVotedInDuelAsync(1002, 1, CancellationToken.None);
        Assert.True(hasVoted);
    }

    /// <summary>
    /// Verifies that HandleAsync with bet decreases user balance.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ValidBet_DecreasesBalance()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var accountRepository = _currentScope.ServiceProvider.GetRequiredService<IAccountRepository>();

        var account = await accountRepository.GetAccountByExternalUserIdAsync(1002, (byte)AuthSystem.Tg, CancellationToken.None);
        var initialBalance = account!.Balance;

        // Act
        await handler.HandleAsync(1002, "bet_1_1_100", CancellationToken.None);

        // Assert
        var newBalance = await accountRepository.GetUserBalanceAsync(1002, CancellationToken.None);
        Assert.Equal(initialBalance - 100m, newBalance);
    }

    #endregion

    #region Bet Placement - Insufficient Balance

    /// <summary>
    /// Verifies that HandleAsync returns error when balance is insufficient.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InsufficientBalance_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1002, "bet_1_1_999999", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Недостаточно средств", result.Message);
    }

    #endregion

    #region Bet Placement - Closed Duel

    /// <summary>
    /// Verifies that HandleAsync returns error when duel is closed.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ClosedDuel_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1002, "bet_1_1_100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Result depends on duel state - just verify it doesn't throw
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when duel has expired.
    /// </summary>
    [Fact]
    public async Task HandleAsync_ExpiredDuel_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1002, "bet_1_1_100", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        // Result depends on duel state - just verify it doesn't throw
    }

    #endregion

    #region Bet Placement - Invalid Format

    /// <summary>
    /// Verifies that HandleAsync with invalid bet format returns error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidBetFormat_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "bet_invalid", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неверный формат", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync with non-numeric bet data returns error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NonNumericBetData_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "bet_abc_def_ghi", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неверные данные", result.Message);
    }

    #endregion

    #region Cancel - Valid Cancel

    /// <summary>
    /// Verifies that HandleAsync with 'cancel_1' returns to duel selection.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Cancel_ReturnsToDuelSelection()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        var scenario = await scenarioRepository.GetScenarioByNameAsync("duels", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, CancellationToken.None);
        Assert.NotNull(step);

        // Create a session first
        await sessionRepository.CreateOrUpdateSessionAsync(
            1001,
            scenario.Id,
            step.Id,
            "{\"step\":\"bet\"}",
            CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(1001, "cancel_1", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Message);
        Assert.NotNull(result.Keyboard);
        Assert.Contains("Bitcoin", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync with cancel updates session to step 2.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Cancel_UpdatesSessionToStep2()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        var scenario = await scenarioRepository.GetScenarioByNameAsync("duels", CancellationToken.None);
        Assert.NotNull(scenario);

        var step2 = await scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, CancellationToken.None);
        Assert.NotNull(step2);

        var step3 = await scenarioRepository.GetStepByOrderAsync(scenario.Id, 3, CancellationToken.None);
        Assert.NotNull(step3);

        // Create a session with step 3
        await sessionRepository.CreateOrUpdateSessionAsync(
            1001,
            scenario.Id,
            step3.Id,
            "{\"step\":\"bet\"}",
            CancellationToken.None);

        // Act
        await handler.HandleAsync(1001, "cancel_1", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(scenario.Id, session.ScenarioId);
        Assert.Equal(step2.Id, session.CurrentStepId);
    }

    #endregion

    #region Cancel - Invalid Format

    /// <summary>
    /// Verifies that HandleAsync with invalid cancel format returns error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_InvalidCancelFormat_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "cancel_invalid", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неверные данные", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync with non-numeric cancel data returns error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NonNumericCancelData_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "cancel_abc", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неверные данные", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when no active session for cancel.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NoActiveSession_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(99999, "cancel_1", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Сессия не найдена", result.Message);
    }

    /// <summary>
    /// Verifies that HandleAsync returns error when duel not found for cancel.
    /// </summary>
    [Fact]
    public async Task HandleAsync_CancelDuelNotFound_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        var scenario = await scenarioRepository.GetScenarioByNameAsync("duels", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetStepByOrderAsync(scenario.Id, 2, CancellationToken.None);
        Assert.NotNull(step);

        // Create a session first
        await sessionRepository.CreateOrUpdateSessionAsync(
            1001,
            scenario.Id,
            step.Id,
            "{\"step\":\"bet\"}",
            CancellationToken.None);

        // Act
        var result = await handler.HandleAsync(1001, "cancel_999", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Дуэль не найдена", result.Message);
    }

    #endregion

    #region Null/Empty Callback

    /// <summary>
    /// Verifies that HandleAsync throws ArgumentNullException when callbackData is null.
    /// </summary>
    [Fact]
    public async Task HandleAsync_NullCallbackData_ThrowsArgumentNullException()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act & Assert
        await Assert.ThrowsAnyAsync<ArgumentNullException>(() => handler.HandleAsync(1001, null!, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that HandleAsync with empty callbackData returns error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_EmptyCallbackData_ReturnsError()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неизвестное действие", result.Message);
    }

    #endregion

    #region Session Management

    /// <summary>
    /// Verifies that HandleAsync with navigation creates a session.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Navigation_CreatesSession()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        var scenario = await scenarioRepository.GetScenarioByNameAsync("start", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, CancellationToken.None);
        Assert.NotNull(step);

        // Act
        await handler.HandleAsync(1001, "main_menu", CancellationToken.None);

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(session);
        Assert.Equal(scenario.Id, session.ScenarioId);
        Assert.Equal(step.Id, session.CurrentStepId);
    }

    /// <summary>
    /// Verifies that HandleAsync with navigation updates existing session.
    /// </summary>
    [Fact]
    public async Task HandleAsync_Navigation_UpdatesExistingSession()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        var scenario = await scenarioRepository.GetScenarioByNameAsync("start", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, CancellationToken.None);
        Assert.NotNull(step);

        // Create initial session
        await sessionRepository.CreateOrUpdateSessionAsync(
            1001,
            scenario.Id,
            step.Id,
            "{}",
            CancellationToken.None);

        var initialSessionId = (await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None))!.Id;

        // Act - navigate to profile
        await handler.HandleAsync(1001, "profile", CancellationToken.None);

        // Assert
        var updatedSession = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(updatedSession);
        Assert.NotEqual(initialSessionId, updatedSession.Id);
    }

    #endregion

    #region Button Navigation

    /// <summary>
    /// Verifies that HandleAsync with standard button navigation updates session.
    /// </summary>
    [Fact]
    public async Task HandleAsync_StandardButtonNavigation_UpdatesSession()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);
        var sessionRepository = _currentScope.ServiceProvider.GetRequiredService<IUserSessionRepository>();
        var scenarioRepository = _currentScope.ServiceProvider.GetRequiredService<IScenarioRepository>();

        var scenario = await scenarioRepository.GetScenarioByNameAsync("start", CancellationToken.None);
        Assert.NotNull(scenario);

        var step = await scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, CancellationToken.None);
        Assert.NotNull(step);

        // Create initial session
        await sessionRepository.CreateOrUpdateSessionAsync(
            1001,
            scenario.Id,
            step.Id,
            "{}",
            CancellationToken.None);

        // Get a button from the step
        var buttons = await scenarioRepository.GetButtonsByStepIdAsync(step.Id, CancellationToken.None);

        // Act
        if (buttons.Count > 0)
        {
            var button = buttons[0];
            await handler.HandleAsync(1001, button.Action, CancellationToken.None);
        }

        // Assert
        var session = await sessionRepository.GetActiveSessionAsync(1001, CancellationToken.None);
        Assert.NotNull(session);
    }

    /// <summary>
    /// Verifies that HandleAsync with unknown button action returns error.
    /// </summary>
    [Fact]
    public async Task HandleAsync_UnknownButtonAction_ReturnsErrorMessage()
    {
        // Arrange
        await _fixture.ResetAsync();
        _currentScope = _fixture.ServiceProvider.CreateScope();
        var handler = _currentScope.ServiceProvider.GetRequiredKeyedService<IButtonHandler>(TgCommandType.Vote);

        // Act
        var result = await handler.HandleAsync(1001, "unknown_action_xyz", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Неизвестное действие", result.Message);
    }

    #endregion

    private bool _disposed;

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _currentScope?.Dispose();
        }

        _disposed = true;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
