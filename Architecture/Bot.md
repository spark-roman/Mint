flowchart TD
    %% --- НАЧАЛО: Инициализация и Главное меню ---
    Start(["/start"]) --> Init["Инициализация пользователя\n+1000 монет"]
    Init --> MainMenu["Главное меню\n(ReplyKeyboard)"]

    MainMenu --> Duels["Сценарий DUELS:\nвыбор категории"]
    MainMenu --> Profile["Сценарий PROFILE:\nпрофиль"]
    MainMenu --> Referral["Сценарий REFERRAL:\nреферальная ссылка"]

    %% --- ПОДГРАФ: Профиль ---
    subgraph ProfileFlow ["👤 Мой профиль"]
        direction TB
        Profile --> ProfileButtons["Кнопки:\n🎁 Бонус | 📈 Лидеры | ⬅️ Назад"]
        
        ProfileButtons --> ClaimBonus["Забрать бонус"]
        ProfileButtons --> Leaderboard["Таблица лидеров\n(топ-15)"]
        ProfileButtons --> BackToMenu1["⬅️ Назад в меню"]

        ClaimBonus --> BonusCheck{"Доступен бонус?"}
        BonusCheck -->|Да| BonusSuccess["Начисление:\n100 + 10×streak"]
        BonusCheck -->|Нет| BonusError["Уведомление:\nбонус недоступен"]

        BonusSuccess --> Profile
        BonusError --> Profile
        Leaderboard --> BackToProfile["🔙 Вернуться в профиль"]
        BackToProfile --> Profile
        BackToMenu1 --> MainMenu
    end

    %% --- ПОДГРАФ: Дуэли ---
    subgraph DuelsFlow ["📊 Дуэли дня"]
        direction TB
        Duels --> Categories["Шаг 1: список категорий"]
        Categories --> DuelCard["Шаг 2: карточка дуэли\n(вопрос + таймер)"]
        
        DuelCard --> BetScreen["Шаг 3: ввод ставки"]
        DuelCard --> ShareDuel["Шеринг дуэли\n(SwitchInlineQuery)"]

        BetScreen --> PlaceBet["Шаг 4: подтверждение ставки"]
        BetScreen --> CancelBet["❌ Отмена"]
        CancelBet --> DuelCard

        PlaceBet -->|Успех| Success["Шаг 5: ставка принята"]
        Success --> NextDuel["📊 Следующая дуэль"]
        Success --> ShareResult["✉️ Переслать результат"]

        NextDuel --> Categories
        ShareResult --> Categories
    end

    %% --- ПОДГРАФ: Рефералка ---
    subgraph ReferralFlow ["👥 Пригласить"]
        direction TB
        Referral --> RefScreen["Реферальная ссылка\nи статистика"]
        RefScreen --> ShareRef["✉️ Переслать ссылку"]
    end

    %% --- СТИЛИ (Цвета) ---
    classDef start fill:#229ED9,stroke:#1d84b5,stroke-width:2px,color:#fff
    classDef menu fill:#ECEFF1,stroke:#607D8B,stroke-width:2px
    classDef screen fill:#E1F5FE,stroke:#03A9F4,stroke-width:2px
    classDef action fill:#FFF9C4,stroke:#FBC02D,stroke-width:2px
    classDef success fill:#E8F5E9,stroke:#4CAF50,stroke-width:2px
    classDef error fill:#FFEBEE,stroke:#EF5350,stroke-width:2px

    class Start,Init start
    class MainMenu menu
    class Duels,Profile,Referral,Categories,DuelCard,BetScreen,Success,Leaderboard,RefScreen screen
    class ClaimBonus,PlaceBet,ShareDuel,ShareRef action
    class BonusSuccess success
    class BonusError error
