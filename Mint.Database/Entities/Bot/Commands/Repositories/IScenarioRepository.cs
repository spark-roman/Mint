using Mint.Database.Entities.Bot.Commands.Dto;

namespace Mint.Database.Entities.Bot.Commands.Repositories;

/// <summary>
/// Provides data access operations for bot scenarios, steps, and buttons
/// </summary>
public interface IScenarioRepository
{
    /// <summary>Retrieves a scenario by its name</summary>
    Task<ScenarioDto?> GetScenarioByNameAsync(string name);

    /// <summary>Retrieves a scenario by its identifier</summary>
    Task<ScenarioDto?> GetScenarioByIdAsync(long id);

    /// <summary>Retrieves all active scenarios</summary>
    Task<List<ScenarioDto>> GetAllScenariosAsync();

    /// <summary>Retrieves a step by its identifier</summary>
    Task<StepDto?> GetStepByIdAsync(long stepId);

    /// <summary>Retrieves the first step of a scenario</summary>
    Task<StepDto?> GetFirstStepByScenarioIdAsync(long scenarioId);

    /// <summary>Retrieves the next step for a button</summary>
    Task<StepDto?> GetNextStepByButtonIdAsync(long buttonId);

    /// <summary>Retrieves all steps of a scenario</summary>
    Task<List<StepDto>> GetStepsByScenarioIdAsync(long scenarioId);

    /// <summary>Retrieves all buttons for a step</summary>
    Task<List<ButtonDto>> GetButtonsByStepIdAsync(long stepId);

    /// <summary>Retrieves a button by its action identifier</summary>
    Task<ButtonDto?> GetButtonByActionAsync(string action);

    /// <summary>Retrieves a button by its identifier</summary>
    Task<ButtonDto?> GetButtonByIdAsync(long buttonId);
}
