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
            SearchKeywords = "DeepSeek, ChatGPT, Midjourney, Claude, Sora AI, генерация видео, ИИ-ассистенты, LLM модели, ИИ в программировании, плагины, автоматизация рутины",
            Code = "ai"
        },
        new CategoryEntity
        {
            Id = 2,
            AiPromptId = 1,
            Name = "Железо & Гаджеты",
            Description = "Анонсы и тесты смартфонов, ноутбуков, процессоров и носимой электроники.",
            IsActiveForAI = true,
            SearchKeywords = "Apple iPhone, Samsung Galaxy, процессоры Nvidia, видеокарты AMD, умные часы, VR/AR гарнитуры, портативные консоли, роутеры, утечки дизайнов, краш-тесты",
            Code = "hardware"
        },
        new CategoryEntity
        {
            Id = 3,
            AiPromptId = 1,
            Name = "Крипта & Web3",
            Description = "Динамика блокчейн-индустрии, курсы монет и экосистема TON.",
            IsActiveForAI = true,
            SearchKeywords = "Курс Bitcoin, экосистема TON, Telegram Wallet, Ethereum, новые аирдропы, мемкоины, стейкинг, криптобиржи, DeFi, аппаратные кошельки, майнинг в РФ",
            Code = "crypto"
        },
        new CategoryEntity
        {
            Id = 4,
            AiPromptId = 1,
            Name = "Видеоигры & Стрим",
            Description = "Релизы на ПК и консолях, индустрия развлечений и тренды игровых платформ.",
            IsActiveForAI = true,
            SearchKeywords = "Steam тренды, PlayStation 5, Xbox, патчи и обновления, GTA 6 слухи, Epic Games Store, популярные стримеры, Twitch, новые трейлеры, инди-игры",
            Code = "games"
        },
        new CategoryEntity
        {
            Id = 5,
            AiPromptId = 1,
            Name = "Киберспорт",
            Description = "Турниры, трансферы, скандалы и результаты в соревновательном гейминге.",
            IsActiveForAI = true,
            SearchKeywords = "The International, PGL Major, Dota 2 трансферы, Counter-Strike 2 матчи, призовые фонды, киберспортивные команды, аналитика матчей, Valorant, League of Legends",
            Code = "sports"
        },
        new CategoryEntity
        {
            Id = 6,
            AiPromptId = 1,
            Name = "Кино & Сериалы",
            Description = "Новинки кинопроката, стриминговые платформы, отзывы и кассовые сборы.",
            IsActiveForAI = true,
            SearchKeywords = "Премьеры кино, новые сериалы, Кинопоиск рейтинг, трейлеры Netflix, HBO, кассовые сборы, актеры, теории фанатов, сиквелы, аниме новинки",
            Code = "movies"
        },
        new CategoryEntity
        {
            Id = 7,
            AiPromptId = 1,
            Name = "Мемы & Рунет",
            Description = "Вирусные тренды, локальные мемы, события в социальных сетях и блогосфере.",
            IsActiveForAI = true,
            SearchKeywords = "Вирусные видео, тренды YouTube, новые шоу, VK Видео, популярные блогеры рунета, интернет-мемы, челленджи, Telegram-тренды, обсуждения в сообществах",
            Code = "memes"
        },
        new CategoryEntity
        {
            Id = 8,
            AiPromptId = 1,
            Name = "Авто & Электрокары",
            Description = "Мировые автомобильные премьеры, технологии и развитие электротранспорта.",
            IsActiveForAI = true,
            SearchKeywords = "Электромобили, Zeekr, Xiaomi SU7, Tesla новости, китайские автобренды, спорткары, концепт-кары, автопилот, тюнинг, гибридные двигатели, тест-драйвы",
            Code = "cars"
        },
        new CategoryEntity
        {
            Id = 9,
            AiPromptId = 1,
            Name = "Наука & Космос",
            Description = "Понятные и интересные открытия, космические миссии и новые технологии.",
            IsActiveForAI = true,
            SearchKeywords = "Запуски SpaceX, телескоп Джеймс Уэбб, Марсианские миссии, квантовые компьютеры, археологические находки, генная инженерия, термоядерный синтез, физика",
            Code = "science"
        },
        new CategoryEntity
        {
            Id = 10,
            AiPromptId = 1,
            Name = "Космос футбола & Спорт",
            Description = "Главные события мирового футбола, боевых искусств и больших спортивных лиг.",
            IsActiveForAI = true,
            SearchKeywords = "Лига Чемпионов, результаты матчей, футбольные трансферы, UFC бои, Формула-1, НХЛ, НБА, громкие рекорды, спортивные скандалы, медиафутбол",
            Code = "soccer"
        },
        new CategoryEntity
        {
            Id = 11,
            AiPromptId = 1,
            Name = "Фэшн & Поп-музыка",
            Description = "Тренды уличной моды, дропы кроссовок, громкие музыкальные альбомы и клипы.",
            IsActiveForAI = true,
            SearchKeywords = "Дропы кроссовок, коллаборации брендов, музыкальные чарты, новые клипы, тренды одежды, рэп-релизы, поп-звезды, музыкальные стриминги, стритвир",
            Code = "fashion"
        },
        new CategoryEntity
        {
            Id = 12,
            AiPromptId = 1,
            Name = "Финтех & Маркетплейсы",
            Description = "Потребительские тренды, новые фичи банков, e-commerce и стартапы.",
            IsActiveForAI = true,
            SearchKeywords = "Обновления Т-Банка, фичи Сбера, тренды Wildberries, Ozon, Мегамаркет, акции ИТ-компаний, новые стартапы, краудфандинг, сервисы доставки, СБП",
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
