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
    public async Task<ScenarioDto?> GetScenarioByNameAsync(string name, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.Scenarios
            .Include(s => s.Steps)
                .ThenInclude(st => st.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Name == name && s.IsActive, cancellationToken);

        return entity != null ? _scenarioMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<ScenarioDto?> GetScenarioByIdAsync(long id, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.Scenarios
            .Include(s => s.Steps)
                .ThenInclude(st => st.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return entity != null ? _scenarioMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<List<ScenarioDto>> GetAllScenariosAsync(CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entities = await context.Scenarios
            .Where(s => s.IsActive)
            .Include(s => s.Steps)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entities.Select(_scenarioMapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<StepDto?> GetStepByIdAsync(long stepId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.Steps
            .Include(st => st.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(st => st.Id == stepId, cancellationToken);

        return entity != null ? _stepMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<StepDto?> GetFirstStepByScenarioIdAsync(long scenarioId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.Steps
            .Include(st => st.Buttons)
            .AsNoTracking()
            .Where(st => st.ScenarioId == scenarioId)
            .OrderBy(st => st.OrderNum)
            .FirstOrDefaultAsync(cancellationToken);

        return entity != null ? _stepMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<StepDto?> GetNextStepByButtonIdAsync(long buttonId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var button = await context.Buttons
            .Include(b => b.NextStep)
                .ThenInclude(s => s!.Buttons)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == buttonId, cancellationToken);

        return button?.NextStep != null ? _stepMapper.Map(button.NextStep) : null;
    }

    /// <inheritdoc />
    public async Task<List<StepDto>> GetStepsByScenarioIdAsync(long scenarioId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entities = await context.Steps
            .Include(st => st.Buttons)
            .AsNoTracking()
            .Where(st => st.ScenarioId == scenarioId)
            .OrderBy(st => st.OrderNum)
            .ToListAsync(cancellationToken);

        return entities.Select(_stepMapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<List<ButtonDto>> GetButtonsByStepIdAsync(long stepId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entities = await context.Buttons
            .AsNoTracking()
            .Where(b => b.ParentStepId == stepId)
            .OrderBy(b => b.OrderNum)
            .ToListAsync(cancellationToken);

        return entities.Select(_buttonMapper.Map).ToList();
    }

    /// <inheritdoc />
    public async Task<ButtonDto?> GetButtonByActionAsync(string action, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.Buttons
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Action == action, cancellationToken);

        return entity != null ? _buttonMapper.Map(entity) : null;
    }

    /// <inheritdoc />
    public async Task<ButtonDto?> GetButtonByIdAsync(long buttonId, CancellationToken cancellationToken)
    {
        await using var context = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        
        var entity = await context.Buttons
            .Include(b => b.NextStep)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == buttonId, cancellationToken);

        return entity != null ? _buttonMapper.Map(entity) : null;
    }
}