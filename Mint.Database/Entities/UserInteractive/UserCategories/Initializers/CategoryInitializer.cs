using System.Collections.ObjectModel;

namespace Mint.Database.Entities.UserInteractive.UserCategories.Initializers;

/// <summary>
/// This class is responsible for seeding the categories table.
/// </summary>
public class CategoryInitializer
{
    private readonly List<CategoryEntity> _categories = 
    [
        new CategoryEntity
        {
            Id = 1,
            AiPromptId = 1,
            Name = "Нейросети & ИИ",
            Description = "Прорывы в сфере искусственного интеллекта, генерации контента и автоматизации.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(нейросеть OR нейросети OR ии OR искусственный интеллект OR chatgpt OR midjourney)",
            Code = "ai"
        },
        new CategoryEntity
        {
            Id = 2,
            AiPromptId = 1,
            Name = "Железо & Гаджеты",
            Description = "Анонсы и тесты смартфонов, ноутбуков, процессоров и носимой электроники.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(смартфон OR процессор OR видеокарта OR iphone OR nvidia OR apple OR intel OR amd)",
            Code = "hardware"
        },
        new CategoryEntity
        {
            Id = 3,
            AiPromptId = 1,
            Name = "Крипта & Web3",
            Description = "Динамика блокчейн-индустрии, курсы монет и экосистема TON.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(крипта OR криптовалюта OR биткоин OR эфириум OR майнинг OR блокчейн OR nft OR токен)",
            Code = "crypto"
        },
        new CategoryEntity
        {
            Id = 4,
            AiPromptId = 1,
            Name = "Видеоигры & Стрим",
            Description = "Релизы на ПК и консолях, индустрия развлечений и тренды игровых платформ.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(гейминг OR стрим OR стример OR twitch OR steam OR playstation OR xbox OR релиз игры)",
            Code = "games"
        },
        new CategoryEntity
        {
            Id = 5,
            AiPromptId = 1,
            Name = "Киберспорт",
            Description = "Турниры, трансферы, скандалы и результаты в соревновательном гейминге.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(киберспорт OR турнир OR дота OR контр страйк OR мажор OR киберспортивный)",
            Code = "sports"
        },
        new CategoryEntity
        {
            Id = 6,
            AiPromptId = 1,
            Name = "Кино & Сериалы",
            Description = "Новинки кинопроката, стриминговые платформы, отзывы и кассовые сборы.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(кинопремьера OR фильм OR сериал OR дорама OR кинотеатр OR нетфликс OR режиссер)",
            Code = "movies"
        },
        new CategoryEntity
        {
            Id = 7,
            AiPromptId = 1,
            Name = "Мемы & Рунет",
            Description = "Вирусные тренды, локальные мемы, события в социальных сетях и блогосфере.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(мем OR мемы OR прикол OR тренд OR блогер OR тикток OR телеграм OR вирусный OR соцсети)",
            Code = "memes"
        },
        new CategoryEntity
        {
            Id = 8,
            AiPromptId = 1,
            Name = "Авто & Электрокары",
            Description = "Мировые автомобильные премьеры, технологии и развитие электротранспорта.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(электрокар OR электромобиль OR тесла OR авторынок OR кроссовер OR zeekr OR гибрид)",
            Code = "cars"
        },
        new CategoryEntity
        {
            Id = 9,
            AiPromptId = 1,
            Name = "Наука & Космос",
            Description = "Понятные и интересные открытия, космические миссии и новые технологии.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(космос OR наса OR роскосмос OR телескоп OR ракета OR запуск OR астрономы OR квантовый)",
            Code = "science"
        },
        new CategoryEntity
        {
            Id = 10,
            AiPromptId = 1,
            Name = "Футбол & Спорт",
            Description = "Главные события мирового футбола, боевых искусств и больших спортивных лиг.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(футбол OR рпл OR лига чемпионов OR трансфер OR спартак OR зенит OR гол OR матч)",
            Code = "soccer"
        },
        new CategoryEntity
        {
            Id = 11,
            AiPromptId = 1,
            Name = "Фэшн & Поп-музыка",
            Description = "Тренды уличной моды, дропы кроссовок, громкие музыкальные альбомы и клипы.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(музыка OR певец OR трек OR альбом OR подиум OR одежда OR фэшн)",
            Code = "fashion"
        },
        new CategoryEntity
        {
            Id = 12,
            AiPromptId = 1,
            Name = "Финтех & Маркетплейсы",
            Description = "Потребительские тренды, новые фичи банков, e-commerce и стартапы.",
            IsActiveForAI = true,
            IsActiveForUsers = true,
            SearchKeywords = "(финтех OR маркетплейс OR маркетплейсы OR озон OR вайлдберриз)",
            Code = "finance"
        }
    ];

    /// <summary>
    /// Returns the list of seed categories.
    /// </summary>
    /// <returns></returns>
    public ReadOnlyCollection<CategoryEntity> Get()
    {
        return new ReadOnlyCollection<CategoryEntity>(_categories);
    }
}
