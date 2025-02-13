using PromotionsEngine.Application.Dtos.Promotion;
using PromotionsEngine.Application.Dtos.PromotionRules;
using PromotionsEngine.Domain.Models;

namespace PromotionsEngine.Application.Mappers;

public static class PromotionsDtoMapper
{
    public static PromotionDto MapToDto(this Promotion domain)
    {
        return new PromotionDto
        {
            Id = domain.Id,
            MerchantId = domain.MerchantId,
            PromotionName = domain.PromotionName,
            PromotionRules = domain.PromotionRules.MapToDto(),
            PromotionType = domain.PromotionTypeEnum?.Name ?? string.Empty,
            PromotionDescription = domain.PromotionDescription,
            PromotionStartDate = domain.PromotionStartDate,
            PromotionEndDate = domain.PromotionEndDate,
            RewardRateType = domain.RewardRateTypeEnum?.Name ?? string.Empty,
            RateAmount = domain.RateAmount,
            Active = domain.Active,
            Deleted = domain.Deleted,
            CreatedDateTime = domain.CreatedDateTime,
            ModifiedDateTime = domain.ModifiedDateTime,
            SchemaVersion = domain.SchemaVersion
        };
    }

    public static PromotionRulesDto MapToDto(this PromotionRules domainRule)
    {
        return new PromotionRulesDto
        {
            NumberOfTimesRedeemable = domainRule.NumberOfTimesRedeemable,
            MinimumTransactionAmount = domainRule.MinimumTransactionAmount,
            MaximumTransactionAmount = domainRule.MaximumTransactionAmount,
            TotalRewardsAmount = domainRule.TotalRewardsAmount,
            NumberOfRedemptionsPerCustomer = domainRule.NumberOfRedemptionsPerCustomer,
            TotalNumberOfCustomers = domainRule.TotalNumberOfCustomers
        };
    }

    public static PromotionRules MapToDomain(this PromotionRulesDto dtoRule)
    {
        return new PromotionRules
        {
            NumberOfTimesRedeemable = dtoRule.NumberOfTimesRedeemable,
            MinimumTransactionAmount = dtoRule.MinimumTransactionAmount,
            MaximumTransactionAmount = dtoRule.MaximumTransactionAmount,
            TotalRewardsAmount = dtoRule.TotalRewardsAmount,
            NumberOfRedemptionsPerCustomer = dtoRule.NumberOfRedemptionsPerCustomer,
            TotalNumberOfCustomers = dtoRule.TotalNumberOfCustomers
        };
    }
}
