using PromotionsEngine.Domain.Models;
using PromotionsEngine.Infrastructure.Entities;

namespace PromotionsEngine.Infrastructure.Mappers;

public static class PromotionSummaryEntityMapper
{
    public static PromotionSummaryEntity MapToEntity(this PromotionSummary promotionSummary)
    {
        return new PromotionSummaryEntity
        {
            Id = promotionSummary.Id,
            NumberOfTimesRedeemed = promotionSummary.NumberOfTimesRedeemed,
            TotalAmountRedeemed = promotionSummary.TotalAmountRedeemed,
            TotalNumberOfCustomers = promotionSummary.TotalNumberOfCustomers
        };
    }

    public static PromotionSummary MapToDomain(this PromotionSummaryEntity entity)
    {
        return new PromotionSummary
        {
            Id = entity.Id,
            NumberOfTimesRedeemed = entity.NumberOfTimesRedeemed,
            TotalAmountRedeemed = entity.TotalAmountRedeemed,
            TotalNumberOfCustomers = entity.TotalNumberOfCustomers
        };
    }
}