using Microsoft.EntityFrameworkCore;
using Mint.Common.Contracts.Mappers;
using Mint.Database.Entities.Bot.Commands.Dto;

namespace Mint.Database.Entities.Bot.Commands.Repositories;

/// <inheritdoc cref="IScenarioRepository"/>
public sealed class ScenarioRepository : IScenarioRepository
{
    private readonly IDbContextFactory<MintDbContext> _dbContextFactory;
    private readonly IDbEntityMapper<ScenarioEntity, ScenarioDto> _scenarioMapper;
    private readonly IDbEntityMapper<StepEntity, StepDto> _stepMapper;
    private readonly IDbEntityMapper<ButtonEntity, ButtonDto> _buttonMapper;

    /// <summary>
    /// Constructor
    /// </summary>
    public ScenarioRepository(
        IDbContextFactory<MintDbContext> dbContextFactory,
        IDbEntityMapper<ScenarioEntity, ScenarioDto> scenarioMapper,
        IDbEntityMapper<StepEntity, StepDto> stepMapper,
        IDbEntityMapper<ButtonEntity, ButtonDto> buttonMapper)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _scenarioMapper = scenarioMapper ?? throw new ArgumentNullException(nameof(scenarioMapper));
        _stepMapper = stepMapper ?? throw new ArgumentNullException(nameof(stepMapper));
        _buttonMapper = buttonMapper ?? throw new ArgumentNullException(nameof(buttonMapper));
    }

    /// <inheritdoc />
    public async Task<ScenarioDto?> GetScenarioByNameAsync(string name)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.Scenarios
            .Include(s => s.Steps)
                .ThenInclude(st => st.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name && s.IsActive);

        return entity != null ? _scenarioMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<ScenarioDto?> GetScenarioByIdAsync(long id)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.Scenarios
            .Include(s => s.Steps)
                .ThenInclude(st => st.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        return entity != null ? _scenarioMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<List<ScenarioDto>> GetAllScenariosAsync()
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entities = await context.Scenarios
            .Where(s => s.IsActive)
            .Include(s => s.Steps)
            .AsNoTracking()
            .ToListAsync();

        return entities.Select(_scenarioMapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<StepDto?> GetStepByIdAsync(long stepId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.Steps
            .Include(st => st.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(st => st.Id == stepId);

        return entity != null ? _stepMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<StepDto?> GetFirstStepByScenarioIdAsync(long scenarioId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.Steps
            .Include(st => st.Buttons)
            .AsNoTracking()
            .Where(st => st.ScenarioId == scenarioId)
            .OrderBy(st => st.OrderNum)
            .FirstOrDefaultAsync();

        return entity != null ? _stepMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<StepDto?> GetNextStepByButtonIdAsync(long buttonId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var button = await context.Buttons
            .Include(b => b.NextStep)
                .ThenInclude(s => s!.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == buttonId);

        return button?.NextStep != null ? _stepMapper.Map(button.NextStep) : null;
    }

    /// <inheritdoc />
    public async Task<List<StepDto>> GetStepsByScenarioIdAsync(long scenarioId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entities = await context.Steps
            .Include(st => st.Buttons)
            .AsNoTracking()
            .Where(st => st.ScenarioId == scenarioId)
            .OrderBy(st => st.OrderNum)
            .ToListAsync();

        return entities.Select(_stepMapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<List<ButtonDto>> GetButtonsByStepIdAsync(long stepId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entities = await context.Buttons
            .AsNoTracking()
            .Where(b => b.ParentStepId == stepId)
            .OrderBy(b => b.OrderNum)
            .ToListAsync();

        return entities.Select(_buttonMapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<ButtonDto?> GetButtonByActionAsync(string action)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.Buttons
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Action == action);

        return entity != null ? _buttonMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<ButtonDto?> GetButtonByIdAsync(long buttonId)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync();
        
        var entity = await context.Buttons
            .Include(b => b.NextStep)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == buttonId);

        return entity != null ? _buttonMapper.Map(entity) : null;
    }
}