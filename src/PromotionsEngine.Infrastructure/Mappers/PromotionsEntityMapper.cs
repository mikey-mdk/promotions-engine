using PromotionsEngine.Domain.Enumerations;
using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;

namespace PromotionsEngine.Infrastructure.Mappers;

public static class PromotionsEntityMapper
{
    public static PromotionEntity MapToEntity(this Promotion domain)
    {
        return new PromotionEntity
        {
            Id = domain.Id,
            MerchantId = domain.MerchantId,
            PromotionName = domain.PromotionName,
            PromotionRules = domain.PromotionRules.MapToEntity(),
            PromotionTypeEnum = domain.PromotionTypeEnum?.Id,
            PromotionDescription = domain.PromotionDescription,
            PromotionStartDate = domain.PromotionStartDate.Date, //using the .date to avoid time of day comparison issues
            PromotionEndDate = domain.PromotionEndDate.Date,
            RewardRateTypeEnum = domain.RewardRateTypeEnum?.Id,
            RateAmount = domain.RateAmount,
            Active = domain.Active,
            Deleted = domain.Deleted,
            CreatedDateTime = domain.CreatedDateTime,
            ModifiedDateTime = domain.ModifiedDateTime,
            CustomerIds = domain.CustomerIds.ToList(),
            SchemaVersion = domain.SchemaVersion,
        };
    }

    public static PromotionRulesEntity MapToEntity(this PromotionRules domainRule)
    {
        return new PromotionRulesEntity
        {
            NumberOfTimesRedeemable = domainRule.NumberOfTimesRedeemable,
            MinimumTransactionAmount = domainRule.MinimumTransactionAmount,
            MaximumTransactionAmount = domainRule.MaximumTransactionAmount,
            TotalRewardsAmount = domainRule.TotalRewardsAmount,
            NumberOfRedemptionsPerCustomer = domainRule.NumberOfRedemptionsPerCustomer,
            TotalNumberOfCustomers = domainRule.TotalNumberOfCustomers
        };
    }

    public static Promotion MapToDomain(this PromotionEntity entity)
    {
        return new Promotion
        {
            Id = entity.Id,
            MerchantId = entity.MerchantId,
            PromotionName = entity.PromotionName,
            PromotionRules = entity.PromotionRules.MapToDomain(),
            PromotionTypeEnum = EnumerationBase.GetAll<PromotionTypeEnum>().FirstOrDefault(x => x.Id == entity.PromotionTypeEnum),
            PromotionDescription = entity.PromotionDescription,
            PromotionStartDate = entity.PromotionStartDate,
            PromotionEndDate = entity.PromotionEndDate,
            RewardRateTypeEnum = EnumerationBase.GetAll<RewardRateTypeEnum>().FirstOrDefault(x => x.Id == entity.RewardRateTypeEnum),
            RateAmount = entity.RateAmount,
            Active = entity.Active,
            Deleted = entity.Deleted,
            CreatedDateTime = entity.CreatedDateTime,
            ModifiedDateTime = entity.ModifiedDateTime,
            CustomerIds = entity.CustomerIds,
            SchemaVersion = entity.SchemaVersion,
        };
    }

    public static PromotionRules MapToDomain(this PromotionRulesEntity entityRule)
    {
        return new PromotionRules
        {
            NumberOfTimesRedeemable = entityRule.NumberOfTimesRedeemable,
            MinimumTransactionAmount = entityRule.MinimumTransactionAmount,
            MaximumTransactionAmount = entityRule.MaximumTransactionAmount,
            TotalRewardsAmount = entityRule.TotalRewardsAmount,
            NumberOfRedemptionsPerCustomer = entityRule.NumberOfRedemptionsPerCustomer,
            TotalNumberOfCustomers = entityRule.TotalNumberOfCustomers
        };
    }
}
