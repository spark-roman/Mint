using System.Collections.ObjectModel;

namespace Mint.Database.Entities.UserInteractive.Bonuses.Initializers;

/// <summary>
/// Initializer for bonus types
/// </summary>
public class BonusTypeInitializer
{
    private readonly BonusTypeEntity[] _bonusTypes =
    [
        new BonusTypeEntity
        {
            Id = 1,
            Code = "start",
            Name = "Стартовый бонус",
            Description = "Бонус за первую регистрацию в боте",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        },
        new BonusTypeEntity
        {
            Id = 2,
            Code = "daily",
            Name = "Ежедневный бонус",
            Description = "Бонус за ежедневный вход в бот",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        },
        new BonusTypeEntity
        {
            Id = 3,
            Code = "streak",
            Name = "Бонус за стрик",
            Description = "Дополнительный бонус за непрерывный стрик 7+ дней",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        },
        new BonusTypeEntity
        {
            Id = 4,
            Code = "referral",
            Name = "Реферальный бонус",
            Description = "Бонус за приведённого друга",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        },
        new BonusTypeEntity
        {
            Id = 5,
            Code = "rating",
            Name = "Рейтинговый бонус",
            Description = "Бонус по итогам голосования",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        },
        new BonusTypeEntity
        {
            Id = 6,
            Code = "admin",
            Name = "Административный бонус",
            Description = "Ручное начисление администратором",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        }
    ];

    /// <summary>
    /// Return bonus types
    /// </summary>
    public ReadOnlyCollection<BonusTypeEntity> Get()
    {
        return new ReadOnlyCollection<BonusTypeEntity>(_bonusTypes);
    }
}
