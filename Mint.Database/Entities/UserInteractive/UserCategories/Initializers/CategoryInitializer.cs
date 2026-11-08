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
            SearchKeywords = "OpenAI ChatGPT Claude Gemini DeepSeek Grok Anthropic xAI LLM ИИ нейросети ИИ-агенты text-to-video Sora анонс обновление release upgrade leak controversy lawsuit ban safety regulation",
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
            SearchKeywords = "Nvidia AMD Intel Apple Samsung Qualcomm Snapdragon PlayStation Xbox Nintendo консоли гаджеты процессор видеокарта чипсет смартфон specs leak benchmark deficit prices overheating",
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
            SearchKeywords = "bitcoin btc ethereum eth solana binance tether usdt ton blockchain крипта криптовалюта Web3 DeFi hack scam regulations SEC санкции market crash dump pump взлом кража листинг",
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
            SearchKeywords = "gaming news Steam PlayStation Xbox Nintendo Switch Twitch YouTube Kick Epic Games релиз патч streamer блогер banned drama controversy scandal бойкот взлом denuvo слив глитч баг",
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
            SearchKeywords = "esports киберспорт Counter-Strike 2 cs2 dota 2 Valorant League of Legends Worlds PGL ESL Valve турнир матч финал мажор major drama disqualification cheat софт скандал замена трансфер бан",
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
            SearchKeywords = "премьера сериал фильм блокбастер тизер трейлер каст актер режиссер Кинопоиск Нетфликс Netflix HBO Кинотеатры дубляж скандал критика разгром провал забастовка цензура спойлер бойкот",
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
            SearchKeywords = "вирусится тренд мем флешмоб паблик каналы блогеры Телеграм Telegram ВКонтакте vk TikTok YouTube рунет Роскомнадзор РКН блокировка замедление сбой штраф запрет скандал хит",
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
            SearchKeywords = "АвтоВАЗ Lada Zeekr LiXiang Xiaomi SU7 Тесла Tesla Москвич автомобили электромобили авторынок дилеры цены параллельный импорт утильсбор авария отзывы брак штрафы дефицит поломка",
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
            SearchKeywords = "space NASA discovery astronomy breakthrough Марс Луна открытие телескоп Джеймс Уэбб SpaceX Роскосмос МКС физика биология археология сигнал аномалия вспышка угроза сенсация опровержение ученые",
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
            SearchKeywords = "спорт футбол хоккей кхл нхл теннис мма ufc бокс фигурные катания рпл лига чемпионов матч судейство скандал разгром драка вар var отмена бойкот пенальти удаление допинг дисквалификация",
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
            SearchKeywords = "pop music fashion week тренд альбом клип показ моды селебрити шоу-бизнес инфлюенсеры Billboard Грэмми Неделя моды celebrity scandal плагиат бойкот отмена критика шок",
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
            SearchKeywords = "wildberries wb ozon мегамаркет яндекс маркет сбер т-банк альфа втб цб рф маркетплейсы финтех банки цифровой рубль онлайн-банкинг штраф скандал комиссия блокировка забастовка сбой мошенники",
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
