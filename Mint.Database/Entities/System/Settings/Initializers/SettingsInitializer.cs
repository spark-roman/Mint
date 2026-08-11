using System.Collections.ObjectModel;

namespace Mint.Database.Entities.System.Settings.Initializers;

/// <summary>
/// This class is responsible for seeding the system settings table.
/// </summary>
public sealed class SettingsInitializer
{
    private readonly List<SystemSettingEntity> _settings =
    [
        new SystemSettingEntity
        {
            Id = 1,
            Key = "StartBonus",
            Value = "5000",
            Description = "Стартовый бонус новому пользователю"
        },
        new SystemSettingEntity
        {
            Id = 2,
            Key = "DailyBonus",
            Value = "1000",
            Description = "Ежедневный бонус"
        },
        new SystemSettingEntity
        {
            Id = 3,
            Key = "StreakBonus",
            Value = "10000",
            Description = "Бонус за стрик 7 дней"
        },
        new SystemSettingEntity
        {
            Id = 4,
            Key = "HouseCommission",
            Value = "0.05",
            Description = "Комиссия при расчете выплаты"
        },
        new SystemSettingEntity
        {
            Id = 5,
            Key = "DuelExpirationHours",
            Value = "24",
            Description = "Время жизни дуэли в часах"
        },
        new SystemSettingEntity
        {
            Id = 6,
            Key = "LeaderboardSize",
            Value = "15",
            Description = "Количество игроков в таблице лидеров"
        },
        new SystemSettingEntity
        {
            Id = 7,
            Key = "MaxBetPercent",
            Value = "100",
            Description = "Максимальный процент от баланса для ставки"
        },
        new SystemSettingEntity
        {
            Id = 8,
            Key = "ReferralBonus",
            Value = "5000",
            Description = "Бонус за приглашенного друга"
        },
        new SystemSettingEntity
        {
            Id = 9,
            Key = "MinBetAmount",
            Value = "10",
            Description = "Минимальная сумма ставки"
        },
        new SystemSettingEntity
        {
            Id = 10,
            Key = "MaxBetAmount",
            Value = "10000",
            Description = "Максимальная сумма ставки"
        }
    ];

    /// <summary>
    /// Returns the list of seed settings.
    /// </summary>
    public ReadOnlyCollection<SystemSettingEntity> Get()
    {
        return new ReadOnlyCollection<SystemSettingEntity>(_settings);
    }
}
