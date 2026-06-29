using System.Collections.ObjectModel;
using Mint.Database.Entities.Bot.Commands;

namespace Mint.Database.Entities.Bot.Commands.Initializers;

/// <summary>
/// This class is responsible for seeding the step_types table.
/// </summary>
public class StepTypeInitializer
{
    private readonly List<StepTypeEntity> _stepTypes = 
    [
        new StepTypeEntity
        {
            Id = 1,
            Name = "text",
            Description = "Ожидается текстовый ввод от пользователя"
        },
        new StepTypeEntity
        {
            Id = 2,
            Name = "number",
            Description = "Ожидается числовой ввод"
        },
        new StepTypeEntity
        {
            Id = 3,
            Name = "choice",
            Description = "Выбор из предложенных вариантов (кнопки)"
        }
    ];

    /// <summary>
    /// Returns the list of seed step types.
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<StepTypeEntity> Get()
    {
        return new ReadOnlyCollection<StepTypeEntity>(_stepTypes);
    }
}
