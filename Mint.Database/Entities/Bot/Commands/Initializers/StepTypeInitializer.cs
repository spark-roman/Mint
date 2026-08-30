using System.Collections.ObjectModel;
using Mint.Common.Contracts.Bot.Commands;

namespace Mint.Database.Entities.Bot.Commands.Initializers;

/// <summary>
/// This class is responsible for seeding the bot scenarios, steps, and buttons.
/// </summary>
public sealed class BotInitializer
{
    private readonly List<StepTypeEntity> _stepTypes =
    [
        new StepTypeEntity { Id = 1, Name = "text", Description = "Expects a text input from the user" },
        new StepTypeEntity { Id = 2, Name = "number", Description = "Expects a numeric input (bet amount)" },
        new StepTypeEntity { Id = 3, Name = "choice", Description = "Selection from suggested options (buttons)" },
        new StepTypeEntity { Id = 4, Name = "info", Description = "Information message without input" }
    ];

    private readonly List<ScenarioEntity> _scenarios =
    [
        new ScenarioEntity { Id = 1, Name = "start", IsActive = true },
        new ScenarioEntity { Id = 2, Name = "profile", IsActive = true },
        new ScenarioEntity { Id = 3, Name = "duels", IsActive = true },
        new ScenarioEntity { Id = 4, Name = "referral", IsActive = true },
        new ScenarioEntity { Id = 5, Name = "leaderboard", IsActive = true },
        new ScenarioEntity { Id = 6, Name = "help", IsActive = true }
    ];

    private readonly List<StepEntity> _steps =
    [
        new StepEntity
        {
            Id = 1,
            ScenarioId = 1,
            OrderNum = 1,
            StepTypeId = 3,
            Message = """
                🎉 Добро пожаловать в "Дуэль мнений"!

                Ваш игровой профиль создан!
                💰 Баланс: {{balance}} 🪙
                🏆 Ранг: {{rank_emoji}} **{{rank_name}}**

                Используйте кнопки ниже для навигации.
                """,
            IsFinal = false
        },
        new StepEntity
        {
            Id = 2,
            ScenarioId = 2,
            OrderNum = 1,
            StepTypeId = 4,
            Message = """
                👤 **Ваш игровой профиль**
                ━━━━━━━━━━━━━━━━━━━━━━━
                🏆 Ранг: {{rank_emoji}} **{{rank_name}}**
                👑 Очки по дуэлям: {{rank_points}}
                💰 Текущие очки: {{balance}} 🪙

                📊 **Статистика прогнозов**
                ├ Всего: {{total_duels}}
                ├ Успешно: {{wins}}
                ├ Неудачно: {{losses}}
                └ Точность: {{winrate}}%

                👥 **Рефералы**
                ├ Приглашено: {{referral_count}}
                └ Всего получено: {{total_referral_bonus}} 🪙

                🎁 **Ежедневный бонус**
                ├ Статус: {{bonus_status}}
                ├ Дней подряд: {{streak_days}} 🔥
                └ Всего получено: {{total_daily_bonus}} 🪙

                📅 В игре с: {{member_since}}
                """,
            IsFinal = false
        },
        new StepEntity
        {
            Id = 3,
            ScenarioId = 3,
            OrderNum = 1,
            StepTypeId = 3,
            Message = "📊 **Выберите категорию споров:**",
            IsFinal = false
        },
        new StepEntity
        {
            Id = 4,
            ScenarioId = 4,
            OrderNum = 1,
            StepTypeId = 4,
            Message = """
                👥 **РЕФЕРАЛЬНАЯ ПРОГРАММА**

                Пригласи друга и получи **{{referral_amount}} монет**!

                🎁 Твоя ссылка:
                `https://t.me/{{bot_username}}?start={{referral_code}}`

                👥 Приглашено друзей: {{referral_count}}
                """,
            IsFinal = false
        },
        new StepEntity
        {
            Id = 5,
            ScenarioId = 6,
            OrderNum = 1,
            StepTypeId = 4,
            Message = """
                🆘 **Помощь по боту "Дуэль мнений"**

                **Как это работает?**

                📊 **Дуэли дня**
                Выберите категорию и сделайте прогноз на актуальный спор.
                Победите, если ваше мнение совпадет с мнением большинства!

                👤 **Мой профиль**
                Следите за своей статистикой, балансом и рангом.
                Забирайте **ежедневный бонус** — он доступен раз в 24 часа.
                Чем дольше вы забираете бонус подряд, тем больше награда за серию!

                👥 **Пригласить**
                Приглашайте друзей по ссылке.
                Когда друг сделает свои первые 3 ставки, вы оба получите бонус!

                💰 **Бонусы:**
                • **Ежедневный:** Доступен 1 раз в 24 часа.
                • **За серию:** Начисляется за 7 дней подряд получения бонуса.
                • **Реферальный:** За приглашенных друзей.

                🏆 **Ранги:**
                Ранг начисляется исходя из очков, полученных за все время игры.
                Чем больше вы участвуете и выигрываете, тем выше ваш ранг.
                Ранг отображается в профиле и дает уважение среди игроков!

                Удачи в спорах! 🍀
                """,
            IsFinal = true
        },
        new StepEntity
        {
            Id = 6,
            ScenarioId = 2,
            OrderNum = 2,
            StepTypeId = 4,
            Message = """
                🏆 **ТАБЛИЦА ЛИДЕРОВ**

                Рейтинг строится на основе **Очков Ранга**.

                {{leaderboard_entries}}

                ───────────────────────
                {{user_rank_info}}
                """,
            IsFinal = false
        },
        new StepEntity
        {
            Id = 7,
            ScenarioId = 3,
            OrderNum = 1,
            StepTypeId = 3,
            Message = "📊 **ДУЭЛИ ДНЯ**\n\nВыберите категорию для спора:{{categories_list}}",
            IsFinal = false
        },
        new StepEntity
        {
            Id = 8,
            ScenarioId = 3,
            OrderNum = 2,
            StepTypeId = 4,
            Message = """
                🤖 **ДУЭЛЬ №{{duel_id}}** (Категория: {{category_name}})
                ───────────────────────
                ❓ **Вопрос:** {{question}}
                
                📝 **Контекст:** {{description}}
                
                ───────────────────────
                ⏱ До закрытия спора: {{time_left}}
                👇 **Сделай свой прогноз:**
                """,
            IsFinal = false
        },
        new StepEntity
        {
            Id = 9,
            ScenarioId = 3,
            OrderNum = 3,
            StepTypeId = 2,
            Message = """
                💰 **ВАШ ПРОГНОЗ: "{{selected_option}}"**

                💳 Ваш текущий баланс: {{balance}} 🪙

                Выберите сумму ставки из шаблонов ниже или введите любое число вручную:
                """,
            IsFinal = false
        },
        new StepEntity
        {
            Id = 10,
            ScenarioId = 3,
            OrderNum = 4,
            StepTypeId = 4,
            Message = """
                ✅ **СТАВКА УСПЕШНО ПРИНЯТА!**

                🎯 Ваш выбор: "{{selected_option}}"
                📉 Сумма спора: {{bet_amount}} 🪙
                ⏳ Расчет дуэли: через {{time_left}}

                Считаешь, что твои друзья в чатах думают иначе?
                Отправь им этот спор, и пускай они попробуют переубедить ИИ.
                """,
            IsFinal = true
        }
    ];

    private readonly List<ButtonEntity> _buttons =
    [
        // ========== start (ParentStepId = 1) ==========
        new ButtonEntity { Id = 1, ParentStepId = 1, OrderNum = 1, Caption = "📊 Дуэли дня", Action = "duels" },
        new ButtonEntity { Id = 2, ParentStepId = 1, OrderNum = 2, Caption = "👤 Мой профиль", Action = "profile" },
        new ButtonEntity { Id = 3, ParentStepId = 1, OrderNum = 3, Caption = "👥 Пригласить", Action = "referral" },
        new ButtonEntity { Id = 11, ParentStepId = 1, OrderNum = 4, Caption = "🆘 Помощь", Action = "help" },

        // ========== profile (ParentStepId = 2) ==========
        new ButtonEntity { Id = 4, ParentStepId = 2, OrderNum = 1, Caption = "🎁 Забрать бонус", Action = "claim_bonus" },
        new ButtonEntity { Id = 5, ParentStepId = 2, OrderNum = 2, Caption = "📈 Таблица лидеров", Action = "leaderboard" },
        new ButtonEntity { Id = 6, ParentStepId = 2, OrderNum = 3, Caption = "🔙 Назад в меню", Action = "main_menu" },

        new ButtonEntity { Id = 12, ParentStepId = 5, OrderNum = 3, Caption = "🔙 Назад в меню", Action = "main_menu" },

        // ========== referral (ParentStepId = 4) ==========
        new ButtonEntity
        {
            Id = 7,
            ParentStepId = 4,
            OrderNum = 1,
            Caption = "✉️ Переслать другу",
            Action = "Присоединяйся к \"Дуэли мнений\" по ссылке: https://t.me/{{bot_username}}?start={{referral_code}}",
            Type = TgButtonType.SwitchInlineQuery
        },
        new ButtonEntity { Id = 13, ParentStepId = 4, OrderNum = 2, Caption = "🔙 Назад в меню", Action = "main_menu" },

        // ========== leaderboard (ParentStepId = 5) ==========
        new ButtonEntity { Id = 8, ParentStepId = 6, OrderNum = 1, Caption = "🔙 Вернуться в профиль", Action = "profile" },

        new ButtonEntity { Id = 9, ParentStepId = 7, OrderNum = 1, Caption = "🔙 Назад в меню", Action = "main_menu" },

        new ButtonEntity { Id = 10, ParentStepId = 8, OrderNum = 1, Caption = "🔙 К дуэлям", Action = "duels" }
    ];

    /// <summary>
    /// Returns the list of step types.
    /// </summary>
#pragma warning disable CA1024 // Use properties where appropriate
    public ReadOnlyCollection<StepTypeEntity> GetStepTypes()
    {
        return new(_stepTypes);
    }

    /// <summary>
    /// Returns the list of scenarios.
    /// </summary>
    public ReadOnlyCollection<ScenarioEntity> GetScenarios()
    {
        return new(_scenarios);
    }

    /// <summary>
    /// Returns the list of steps.
    /// </summary>
    public ReadOnlyCollection<StepEntity> GetSteps()
    {
        return new(_steps);
    }

    /// <summary>
    /// Returns the list of buttons.
    /// </summary>
    public ReadOnlyCollection<ButtonEntity> GetButtons()
    {
        return new(_buttons);
    }
#pragma warning restore CA1024 // Use properties where appropriate
}