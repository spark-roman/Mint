using Mint.App.Services.System.Bot.Dto;
using Mint.App.Services.System.Bot.Handlers.Messages;
using Mint.Database.Entities.Bot.Commands.Dto;
using Mint.Database.Entities.Bot.Commands.Repositories;
using Mint.Database.Entities.UserInteractive.UserCategories.Repositories;
using Mint.Database.Entities.Users.Sessions.Repositories;
using Telegram.Bot.Types;

namespace Mint.App.Services.System.Bot.Handlers.Commands;

/// <inheritdoc cref="ICommandHandler"/>
public sealed class DuelsCommandHandler(
    IScenarioRepository scenarioRepository,
    IUserSessionRepository sessionRepository,
    ICategoryRepository categoryRepository) : ICommandHandler
{
    private readonly IScenarioRepository _scenarioRepository = scenarioRepository;
    private readonly IUserSessionRepository _sessionRepository = sessionRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    /// <inheritdoc />
    public async Task<CommandResult> HandleAsync(User tgUser, string command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(tgUser);

        var scenario = await _scenarioRepository.GetScenarioByNameAsync("duels", cancellationToken);
        if (scenario == null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: сценарий 'duels' не найден",
                IsFinal = true
            };
        }

        var step = await _scenarioRepository.GetFirstStepByScenarioIdAsync(scenario.Id, cancellationToken);
        if (step == null)
        {
            return new CommandResult
            {
                Message = "❌ Ошибка: первый шаг не найден",
                IsFinal = true
            };
        }

        var categories = await _categoryRepository.GetAllActiveAsync(cancellationToken);
        var categoryButtons = categories
            .Select(c => new ButtonDto
            {
                Caption = $"📂 {c.Name}",
                Action = $"category_{c.Code}",
                OrderNum = (short)Array.IndexOf(categories.ToArray(), c)
            })
            .ToList();

        await _sessionRepository.CreateOrUpdateSessionAsync(
            tgUser.Id,
            scenario.Id,
            step.Id,
            "{\"step\":\"categories\"}",
            cancellationToken);

        return new CommandResult
        {
            Message = step.Message,
            Keyboard = categoryButtons.AsReadOnly(),
            IsFinal = step.IsFinal,
            IsNewMessage = false
        };
    }
}
