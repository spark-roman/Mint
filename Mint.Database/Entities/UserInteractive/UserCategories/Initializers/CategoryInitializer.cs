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
            SearchKeywords = "OpenAI ChatGPT Claude Gemini xAI ИИ нейросети release upgrade leak controversy lawsuit ban safety",
            Code = "ai"
        },
        new CategoryEntity
        {
            Id = 2,
            AiPromptId = 1,
            Name = "Железо & Гаджеты",
            Description = "Анонсы и тесты смартфонов, ноутбуков, процессоров и носимой электроники.",
            IsActiveForAI = true,
            SearchKeywords = "Nvidia AMD Intel Apple Samsung hardware specs smartphone gpu release leak benchmark deficit prices",
            Code = "hardware"
        },
        new CategoryEntity
        {
            Id = 3,
            AiPromptId = 1,
            Name = "Крипта & Web3",
            Description = "Динамика блокчейн-индустрии, курсы монет и экосистема TON.",
            IsActiveForAI = true,
            SearchKeywords = "bitcoin crypto ethereum hack binance sec scam regulations market crash dump взлом",
            Code = "crypto"
        },
        new CategoryEntity
        {
            Id = 4,
            AiPromptId = 1,
            Name = "Видеоигры & Стрим",
            Description = "Релизы на ПК и консолях, индустрия развлечений и тренды игровых платформ.",
            IsActiveForAI = true,
            SearchKeywords = "gaming news Twitch streamer banned drama controversy game release patch Steam взлом слив",
            Code = "games"
        },
        new CategoryEntity
        {
            Id = 5,
            AiPromptId = 1,
            Name = "Киберспорт",
            Description = "Турниры, трансферы, скандалы и результаты в соревновательном гейминге.",
            IsActiveForAI = true,
            SearchKeywords = "esports cs2 dota2 tournament match drama disqualification Valve cheat софт скандал",
            Code = "sports"
        },
        new CategoryEntity
        {
            Id = 6,
            AiPromptId = 1,
            Name = "Кино & Сериалы",
            Description = "Новинки кинопроката, стриминговые платформы, отзывы и кассовые сборы.",
            IsActiveForAI = true,
            SearchKeywords = "премьера кинотеатры сериал кинопоиск трейлер нетфликс скандал критика провал режиссер актеры",
            Code = "movies"
        },
        new CategoryEntity
        {
            Id = 7,
            AiPromptId = 1,
            Name = "Мемы & Рунет",
            Description = "Вирусные тренды, локальные мемы, события в социальных сетях и блогосфере.",
            IsActiveForAI = true,
            SearchKeywords = "вирусится тренд мем ркн блокировка телеграм вконтакте тикток сбой штраф",
            Code = "memes"
        },
        new CategoryEntity
        {
            Id = 8,
            AiPromptId = 1,
            Name = "Авто & Электрокары",
            Description = "Мировые автомобильные премьеры, технологии и развитие электротранспорта.",
            IsActiveForAI = true,
            SearchKeywords = "автоваз lada автомобили параллельный импорт цены zeekr lixiang электромобили авария утильсбор",
            Code = "cars"
        },
        new CategoryEntity
        {
            Id = 9,
            AiPromptId = 1,
            Name = "Наука & Космос",
            Description = "Понятные и интересные открытия, космические миссии и новые технологии.",
            IsActiveForAI = true,
            SearchKeywords = "space NASA discovery astronomy breakthrough SpaceX Марс ученые открытие аномалия сигнал",
            Code = "science"
        },
        new CategoryEntity
        {
            Id = 10,
            AiPromptId = 1,
            Name = "Футбол & Спорт",
            Description = "Главные события мирового футбола, боевых искусств и больших спортивных лиг.",
            IsActiveForAI = true,
            SearchKeywords = "рпл футбол судейство скандал матч рфс лига чемпионов гол разгром трансфер drama var",
            Code = "soccer"
        },
        new CategoryEntity
        {
            Id = 11,
            AiPromptId = 1,
            Name = "Фэшн & Поп-музыка",
            Description = "Тренды уличной моды, дропы кроссовок, громкие музыкальные альбомы и клипы.",
            IsActiveForAI = true,
            SearchKeywords = "pop music billboard fashion week celebrity scandal Грэмми показ моды тренд плагиат",
            Code = "fashion"
        },
        new CategoryEntity
        {
            Id = 12,
            AiPromptId = 1,
            Name = "Финтех & Маркетплейсы",
            Description = "Потребительские тренды, новые фичи банков, e-commerce и стартапы.",
            IsActiveForAI = true,
            SearchKeywords = "wildberries ozon сбер вб штраф скандал комиссия финтех банки цб рф цифровой рубль сбой",
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
