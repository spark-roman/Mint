using System.Collections.ObjectModel;

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
        new ScenarioEntity { Id = 5, Name = "leaderboard", IsActive = true } // ← добавляем
    ];

    private readonly List<StepEntity> _steps =
    [
        // ========== SCENARIO: start (Id = 1) ==========
        new StepEntity
        {
            Id = 1,
            ScenarioId = 1,
            OrderNum = 1,
            StepTypeId = 3,
            Message = """
                🎉 Добро пожаловать в "Дуэль мнений"!

                Ваш игровой профиль создан!
                💰 Стартовый баланс: {{balance}} 🪙
                🏆 Ранг: {{rank_emoji}} **{{rank_name}}**

                Используйте кнопки ниже для навигации.
                """,
            IsFinal = false
        },

        // ========== SCENARIO: profile (Id = 2) ==========
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

        // ========== SCENARIO: duels (Id = 3) ==========
        new StepEntity
        {
            Id = 3,
            ScenarioId = 3,
            OrderNum = 1,
            StepTypeId = 3,
            Message = "📊 **Выберите категорию споров:**",
            IsFinal = false
        },

        // ========== SCENARIO: referral (Id = 4) ==========
        new StepEntity
        {
            Id = 4,
            ScenarioId = 4,
            OrderNum = 1,
            StepTypeId = 4,
            Message = """
                👥 **РЕФЕРАЛЬНАЯ ПРОГРАММА**

                Пригласи друга и получи **500 монет**, когда он сделает свои первые 3 ставки в любых дуэлях!

                🎁 Твоя ссылка:
                `https://t.me/opinion_bot?start={{referral_code}}`

                👥 Приглашено друзей: {{referral_count}}
                💰 Всего бонусов: {{total_referral_bonus}} 🪙
                """,
            IsFinal = false
        },

        // ========== Шаг 2: Лидерборд (добавляем) ==========
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
        }
    ];

    private readonly List<ButtonEntity> _buttons =
    [
        // ========== start (ParentStepId = 1) ==========
        new ButtonEntity { Id = 1, ParentStepId = 1, OrderNum = 1, Caption = "📊 Дуэли дня", Action = "duels" },
        new ButtonEntity { Id = 2, ParentStepId = 1, OrderNum = 2, Caption = "👤 Мой профиль", Action = "profile" },
        new ButtonEntity { Id = 3, ParentStepId = 1, OrderNum = 3, Caption = "👥 Пригласить", Action = "referral" },

        // ========== profile (ParentStepId = 2) ==========
        new ButtonEntity { Id = 4, ParentStepId = 2, OrderNum = 1, Caption = "🎁 Забрать бонус", Action = "claim_bonus" },
        new ButtonEntity { Id = 5, ParentStepId = 2, OrderNum = 2, Caption = "📈 Таблица лидеров", Action = "leaderboard" },
        new ButtonEntity { Id = 6, ParentStepId = 2, OrderNum = 3, Caption = "⬅️ Назад в меню", Action = "main_menu" },

        // ========== referral (ParentStepId = 4) ==========
        new ButtonEntity { Id = 7, ParentStepId = 4, OrderNum = 1, Caption = "✉️ Переслать другу", Action = "share_referral" },

        // ========== leaderboard (ParentStepId = 5) ==========
        new ButtonEntity { Id = 8, ParentStepId = 6, OrderNum = 1, Caption = "🔙 Вернуться в профиль", Action = "profile" }
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