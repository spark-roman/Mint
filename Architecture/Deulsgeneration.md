flowchart TD
    subgraph Scheduler1["⏰ Расписание 1 (06:00 UTC)"]
        A["Запуск сбора новостей"]
    end

    subgraph Scheduler2["⏰ Расписание 2 (07:00 UTC)"]
        M["Запуск генерации дуэлей"]
    end

    subgraph NewsProcess["📰 Сбор новостей"]
        B["Загрузка RSS-лент<br>(Sagara.FeedReader)"] --> C["Сохранение в БД<br>(NewsEntity)"]
    end

    subgraph DuelProcess["⚔️ Генерация дуэлей"]
        D["Выборка необработанных<br>новостей (до 50 шт.)"] --> E{"Есть новости<br>для обработки?"}
        E -->|Нет| F["Завершение<br>(дуэлей нет)"]
        E -->|Да| G["Формирование промпта"]
        G --> H["Отправка в DeepSeek API"]
        H --> I{"Валидный JSON?"}
        I -->|Да| J["Сохранение дуэлей в БД"]
        I -->|Нет| K["Повторная попытка<br>с корректировкой"]
        J --> L["Отметка новостей<br>как обработанных"]
        L --> D
        K --> D
    end

    subgraph Notification["🔔 Оповещение"]
        O["Пользователи видят<br>новые дуэли"]
    end

    Scheduler1 --> NewsProcess
    NewsProcess --> Scheduler2
    Scheduler2 --> DuelProcess
    DuelProcess --> Notification

    classDef default fill:#f9f9f9,stroke:#333,stroke-width:1px
    classDef scheduler fill:#E1F5FE,stroke:#03A9F4,stroke-width:2px
    classDef news fill:#FFF9C4,stroke:#FBC02D,stroke-width:2px
    classDef duel fill:#E8F5E9,stroke:#4CAF50,stroke-width:2px
    classDef notify fill:#F3E5F5,stroke:#9C27B0,stroke-width:2px

    class Scheduler1,Scheduler2 scheduler
    class NewsProcess news
    class DuelProcess duel
    class Notification notify
