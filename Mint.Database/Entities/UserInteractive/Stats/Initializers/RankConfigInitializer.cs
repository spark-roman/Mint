using System.Collections.ObjectModel;
using Mint.Database.Entities.UserInteractive.Stats;

namespace Mint.Database.Entities.UserInteractive.Stats.Initializers;

/// <summary>
/// Initializes the rank config data for the database.
/// </summary>
public class RankConfigInitializer
{
    private readonly List<RankConfigEntity> _ranks = 
    [
        new RankConfigEntity
        {
            Id = 1,
            Code = "newbie",
            Name = "Новичок",
            Emoji = "🥚",
            MinPoints = 0
        },
        new RankConfigEntity
        {
            Id = 2,
            Code = "analyst",
            Name = "Аналитик",
            Emoji = "📈",
            MinPoints = 20
        },
        new RankConfigEntity
        {
            Id = 3,
            Code = "expert",
            Name = "Эксперт",
            Emoji = "🧠",
            MinPoints = 100
        },
        new RankConfigEntity
        {
            Id = 4,
            Code = "seer",
            Name = "Провидец",
            Emoji = "🔮",
            MinPoints = 500
        },
        new RankConfigEntity
        {
            Id = 5,
            Code = "oracle",
            Name = "Оракул",
            Emoji = "👁️",
            MinPoints = 2000
        }
    ];

    /// <summary>
    /// Returns the list of seed rank configs.
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<RankConfigEntity> Get()
    {
        return new ReadOnlyCollection<RankConfigEntity>(_ranks);
    }
}
