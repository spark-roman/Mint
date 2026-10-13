using Mint.Common.Contracts.Mappers;
using Mint.Common.Contracts.UserInteractive.Bonuses;
using Mint.Database.Entities.System.Payouts.Dto;
using Mint.Database.Entities.UserInteractive.Stats.Dto;

namespace Mint.App.Services.System.WinCalculation.Mappers;

/// <inheritdoc/>
public class PayoutStatsMapper : IDtoMapper<PayoutDto, UserStatsUpdateDto>
{
    /// <inheritdoc/>
    public UserStatsUpdateDto Map(PayoutDto dto, params object[] args)
    {
        ArgumentNullException.ThrowIfNull(dto);
        ArgumentNullException.ThrowIfNull(args);

        var userStats = args[0] as UserStatsDto ?? throw new InvalidOperationException("User stats not found");

        var statsUpdateDto = dto.BonusType switch
        {
            BonusType.Payout => new UserStatsUpdateDto
            {
                RankPoints = userStats.RankPoints + dto.Amount,
                TotalWins = userStats.TotalWins + 1,
                TotalLosses = userStats.TotalLosses,
                TotalDraws = userStats.TotalDraws,
                ReferralCount = userStats.ReferralCount
            },
            BonusType.Refund => new UserStatsUpdateDto
            {
                RankPoints = userStats.RankPoints,
                TotalWins = userStats.TotalWins,
                TotalLosses = userStats.TotalLosses,
                TotalDraws = userStats.TotalDraws + 1,
                ReferralCount = userStats.ReferralCount
            },
            BonusType.NoActivity => new UserStatsUpdateDto
            {
                RankPoints = userStats.RankPoints,
                TotalWins = userStats.TotalWins,
                TotalLosses = userStats.TotalLosses + 1,
                TotalDraws = userStats.TotalDraws,
                ReferralCount = userStats.ReferralCount
            },
            _ => throw new InvalidOperationException("Invalid bonus type")
        };

        return statsUpdateDto;
    }
}
